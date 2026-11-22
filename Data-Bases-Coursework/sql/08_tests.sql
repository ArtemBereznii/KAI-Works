-- 08_tests.sql — тестові сценарії T01–T32 (форма протоколу — Додаток Е).
-- Кожен тест записує в тимчасовий журнал очікуваний і фактичний результат.
-- Усі тести виконуються в одній транзакції, яка завершується ROLLBACK:
-- дані наповнення після тестів залишаються незмінними.
-- Якщо хоча б один тест не пройдено, скрипт завершується помилкою.
-- Нові картки для тестів мають номери 4999…; моменти операцій — 25.11.2026,
-- тобто після періоду наповнення.
-- Конкурентний доступ (BR11) перевіряється окремо у двох сеансах psql: protocols/concurrency.md.

SET search_path = transit, public;
SET timezone = 'Europe/Kyiv';
\pset null '—'

CREATE TEMP TABLE test_log (
    code      text PRIMARY KEY,
    rule      text,
    action    text,
    expected  text,
    actual    text,
    passed    boolean
);

-- Запис результату тесту
CREATE FUNCTION pg_temp.t_log(p_code text, p_rule text, p_action text,
                              p_expected text, p_actual text, p_passed boolean)
RETURNS void LANGUAGE sql
AS $$
    INSERT INTO test_log VALUES (p_code, p_rule, p_action, p_expected, p_actual, COALESCE(p_passed, false));
$$;

-- Негативний тест: виконує команду (за потреби від імені ролі) і порівнює SQLSTATE
-- та початок повідомлення з очікуваними. Невдала команда відкочується разом із підтранзакцією.
CREATE FUNCTION pg_temp.expect_error(p_code text, p_rule text, p_action text, p_sql text,
                                     p_state text, p_msg_like text DEFAULT NULL,
                                     p_role text DEFAULT NULL)
RETURNS void LANGUAGE plpgsql
AS $$
DECLARE
    v_state text;
    v_msg   text;
BEGIN
    BEGIN
        IF p_role IS NOT NULL THEN
            EXECUTE format('SET LOCAL ROLE %I', p_role);
        END IF;
        EXECUTE p_sql;
        v_state := '00000';
        v_msg   := 'виконано без помилки';
    EXCEPTION WHEN OTHERS THEN
        GET STACKED DIAGNOSTICS v_state = RETURNED_SQLSTATE, v_msg = MESSAGE_TEXT;
    END;
    RESET ROLE;
    PERFORM pg_temp.t_log(p_code, p_rule, p_action,
        p_state || COALESCE(' ' || p_msg_like, ''),
        v_state || ' ' || v_msg,
        v_state = p_state AND (p_msg_like IS NULL OR v_msg LIKE p_msg_like || '%'));
END;
$$;

BEGIN;

-- Видача картки та формат номера (BR01)
DO $$
DECLARE v_id integer; v_card cards%ROWTYPE; v_hist integer;
BEGIN
    CALL proc_issue_card('4999000000000001', NULL, v_id, NULL, '2026-11-25 08:00');
    SELECT * INTO v_card FROM cards WHERE id = v_id;
    SELECT count(*) INTO v_hist FROM card_status_history
    WHERE card_id = v_id AND old_status IS NULL AND new_status = 'active' AND reason = 'Видача картки';
    PERFORM pg_temp.t_log('T01', 'BR01', 'Видача анонімної картки 4999000000000001',
        'active, баланс 0.00, діє 5 років, 1 запис історії',
        format('%s, баланс %s, діє до %s, записів історії %s', v_card.status, v_card.balance, v_card.valid_until, v_hist),
        v_card.status = 'active' AND v_card.balance = 0 AND v_card.valid_until = DATE '2031-11-25' AND v_hist = 1);
END $$;

SELECT pg_temp.expect_error('T02', 'BR01', 'Повторна видача картки з тим самим номером',
    $q$CALL proc_issue_card('4999000000000001', NULL, NULL)$q$, '23505');
SELECT pg_temp.expect_error('T03', 'BR01', 'Видача картки з номером 12AB',
    $q$CALL proc_issue_card('12AB', NULL, NULL)$q$, '23514');

-- Межі суми поповнення (BR03)
DO $$
DECLARE v_id bigint; v_bal numeric;
BEGIN
    CALL proc_top_up('4999000000000001', 1,    'cash_desk', 'TEST-001', v_id, v_bal, '2026-11-25 08:05');
    CALL proc_top_up('4999000000000001', 5000, 'online',    'TEST-002', v_id, v_bal, '2026-11-25 08:06');
    PERFORM pg_temp.t_log('T04', 'BR03', 'Поповнення на граничні суми 1 і 5000',
        'обидва прийнято, баланс 5001.00', 'баланс ' || v_bal, v_bal = 5001);
END $$;

SELECT pg_temp.expect_error('T05', 'BR03', 'Поповнення на 0.99',
    $q$CALL proc_top_up('4999000000000001', 0.99, 'online', 'TEST-003', NULL, NULL, '2026-11-25 08:07')$q$, '23514');
SELECT pg_temp.expect_error('T06', 'BR03', 'Поповнення на 5000.01',
    $q$CALL proc_top_up('4999000000000001', 5000.01, 'online', 'TEST-004', NULL, NULL, '2026-11-25 08:07')$q$, '23514');

-- Ідемпотентність платежу (BR09)
DO $$
DECLARE v_id bigint; v_bal numeric; v_rows integer;
BEGIN
    CALL proc_top_up('4999000000000001', 1, 'cash_desk', 'TEST-001', v_id, v_bal, '2026-11-25 08:10');
    SELECT count(*) INTO v_rows FROM top_ups WHERE payment_ref = 'TEST-001';
    PERFORM pg_temp.t_log('T07', 'BR09', 'Повтор платежу TEST-001 (та сама картка й сума)',
        '1 запис TEST-001, баланс 5001.00',
        format('записів %s, баланс %s', v_rows, (SELECT balance FROM cards WHERE card_number = '4999000000000001')),
        v_rows = 1 AND (SELECT balance FROM cards WHERE card_number = '4999000000000001') = 5001);
END $$;

SELECT pg_temp.expect_error('T08', 'BR09', 'Платіж TEST-001 для іншої картки',
    $q$CALL proc_top_up('4000000000000001', 1, 'cash_desk', 'TEST-001', NULL, NULL, '2026-11-25 08:11')$q$,
    'P0001', 'BR09');

-- Оплата поїздок і безкоштовна пересадка (BR10)
-- Тариф з 01.11: автобус 20.00, метро 25.00. V-10001, V-10002 — автобус, V-40001 — метро.
DO $$
DECLARE v_id bigint; v_m text; v_amt numeric; v_bal numeric; v_charges integer;
BEGIN
    CALL proc_register_trip('4999000000000001', 'V-10001', v_id, v_m, v_amt, v_bal, '2026-11-25 09:00');
    SELECT count(*) INTO v_charges FROM charges WHERE trip_id = v_id;
    PERFORM pg_temp.t_log('T09', 'FR03', 'Поїздка автобусом о 09:00 з балансу',
        'balance, списано 20.00, баланс 4981.00, 1 списання',
        format('%s, списано %s, баланс %s, списань %s', v_m, v_amt, v_bal, v_charges),
        v_m = 'balance' AND v_amt = 20 AND v_bal = 4981 AND v_charges = 1);

    CALL proc_register_trip('4999000000000001', 'V-40001', v_id, v_m, v_amt, v_bal, '2026-11-25 09:30');
    PERFORM pg_temp.t_log('T10', 'BR10', 'Метро через 30 хв після платної поїздки',
        'transfer, списано 0, баланс 4981.00',
        format('%s, списано %s, баланс %s', v_m, v_amt, v_bal),
        v_m = 'transfer' AND v_amt = 0 AND v_bal = 4981);

    CALL proc_register_trip('4999000000000001', 'V-10002', v_id, v_m, v_amt, v_bal, '2026-11-25 09:45');
    PERFORM pg_temp.t_log('T11', 'BR10', 'Друга пересадка від тієї самої платної поїздки (09:45)',
        'balance, списано 20.00 (пересадку вже використано)',
        format('%s, списано %s, баланс %s', v_m, v_amt, v_bal),
        v_m = 'balance' AND v_amt = 20 AND v_bal = 4961);

    CALL proc_register_trip('4999000000000001', 'V-40001', v_id, v_m, v_amt, v_bal, '2026-11-25 10:50');
    PERFORM pg_temp.t_log('T12', 'BR10', 'Метро через 65 хв після платної поїздки',
        'balance, списано 25.00 (поза вікном 60 хв)',
        format('%s, списано %s, баланс %s', v_m, v_amt, v_bal),
        v_m = 'balance' AND v_amt = 25 AND v_bal = 4936);

    CALL proc_register_trip('4999000000000001', 'V-40001', v_id, v_m, v_amt, v_bal, '2026-11-25 11:00');
    PERFORM pg_temp.t_log('T13', 'BR10', 'Повторна валідація на тому самому валідаторі через 10 хв',
        'balance, списано 25.00 (пересадка лише на інший валідатор)',
        format('%s, списано %s, баланс %s', v_m, v_amt, v_bal),
        v_m = 'balance' AND v_amt = 25 AND v_bal = 4911);
END $$;

SELECT pg_temp.expect_error('T14', 'BR15', 'Друга валідація картки в той самий момент (11:00)',
    $q$CALL proc_register_trip('4999000000000001', 'V-10001', NULL, NULL, NULL, NULL, '2026-11-25 11:00')$q$, '23505');

-- Недостатньо коштів: процедура не залишає часткових змін (BR02, атомарність)
DO $$
DECLARE v_id integer; v_tid bigint; v_bal numeric; v_m text; v_amt numeric; v_state text; v_msg text;
BEGIN
    CALL proc_issue_card('4999000000000002', NULL, v_id, NULL, '2026-11-25 08:00');
    CALL proc_top_up('4999000000000002', 10, 'terminal', 'TEST-010', v_tid, v_bal, '2026-11-25 08:01');
    BEGIN
        CALL proc_register_trip('4999000000000002', 'V-10001', v_tid, v_m, v_amt, v_bal, '2026-11-25 12:00');
        v_state := '00000'; v_msg := 'виконано без помилки';
    EXCEPTION WHEN OTHERS THEN
        GET STACKED DIAGNOSTICS v_state = RETURNED_SQLSTATE, v_msg = MESSAGE_TEXT;
    END;
    PERFORM pg_temp.t_log('T15', 'BR02', 'Поїздка автобусом (20.00) з балансом 10.00',
        'P0001 BR02; поїздок 0, списань 0, баланс 10.00',
        format('%s %s; поїздок %s, списань %s, баланс %s', v_state, v_msg,
               (SELECT count(*) FROM trips WHERE card_id = v_id),
               (SELECT count(*) FROM charges WHERE card_id = v_id),
               (SELECT balance FROM cards WHERE id = v_id)),
        v_state = 'P0001' AND v_msg LIKE 'BR02%'
        AND NOT EXISTS (SELECT 1 FROM trips WHERE card_id = v_id)
        AND NOT EXISTS (SELECT 1 FROM charges WHERE card_id = v_id)
        AND (SELECT balance FROM cards WHERE id = v_id) = 10);
END $$;

-- Проїзні (BR07)
DO $$
DECLARE v_sid integer; v_amt numeric; v_bal numeric; v_tid bigint; v_m text;
BEGIN
    CALL proc_buy_plan('4999000000000001', 'M30_ALL', v_sid, v_amt, v_bal, '2026-11-25 12:00');
    PERFORM pg_temp.t_log('T16', 'BR07', 'Купівля M30_ALL з балансом 4911.00',
        'списано 1200.00, баланс 3711.00, проїзний до 25.12 12:00',
        format('списано %s, баланс %s, до %s', v_amt, v_bal, (SELECT valid_to FROM card_subscriptions WHERE id = v_sid)),
        v_amt = 1200 AND v_bal = 3711
        AND (SELECT valid_to FROM card_subscriptions WHERE id = v_sid) = timestamptz '2026-12-25 12:00');

    CALL proc_register_trip('4999000000000001', 'V-10001', v_tid, v_m, v_amt, v_bal, '2026-11-25 12:30');
    PERFORM pg_temp.t_log('T17', 'BR13', 'Поїздка автобусом під час дії M30_ALL',
        'subscription, списано 0, посилання на проїзний',
        format('%s, списано %s, subscription_id %s', v_m, v_amt, (SELECT subscription_id FROM trips WHERE id = v_tid)),
        v_m = 'subscription' AND v_amt = 0 AND (SELECT subscription_id FROM trips WHERE id = v_tid) = v_sid);
END $$;

SELECT pg_temp.expect_error('T18', 'BR07', 'Купівля W7_ALL, що перетинається з M30_ALL',
    $q$CALL proc_buy_plan('4999000000000001', 'W7_ALL', NULL, NULL, NULL, '2026-11-25 12:05')$q$, '23P01');

-- Статуси картки (BR04, BR05)
DO $$
DECLARE v_state text; v_msg text; v_tid bigint; v_m text; v_amt numeric; v_bal numeric;
BEGIN
    CALL proc_set_card_status('4999000000000001', 'blocked', 'Тест T19');
    BEGIN
        CALL proc_register_trip('4999000000000001', 'V-10001', v_tid, v_m, v_amt, v_bal, '2026-11-25 13:00');
        v_state := '00000'; v_msg := 'виконано без помилки';
    EXCEPTION WHEN OTHERS THEN
        GET STACKED DIAGNOSTICS v_state = RETURNED_SQLSTATE, v_msg = MESSAGE_TEXT;
    END;
    PERFORM pg_temp.t_log('T19', 'BR04', 'Поїздка заблокованою карткою',
        'P0001 BR04', v_state || ' ' || v_msg, v_state = 'P0001' AND v_msg LIKE 'BR04%');

    CALL proc_set_card_status('4999000000000001', 'active', 'Тест T20');
    PERFORM pg_temp.t_log('T20', 'BR05', 'Розблокування картки (blocked → active)',
        'статус active, 3 записи історії',
        format('статус %s, записів історії %s',
               (SELECT status FROM cards WHERE card_number = '4999000000000001'),
               (SELECT count(*) FROM card_status_history h JOIN cards c ON c.id = h.card_id
                WHERE c.card_number = '4999000000000001')),
        (SELECT status FROM cards WHERE card_number = '4999000000000001') = 'active'
        AND (SELECT count(*) FROM card_status_history h JOIN cards c ON c.id = h.card_id
             WHERE c.card_number = '4999000000000001') = 3);
END $$;

SELECT pg_temp.expect_error('T21', 'BR05', 'Повернення втраченої картки 4000000000000014 у статус active',
    $q$CALL proc_set_card_status('4000000000000014', 'active', 'Тест')$q$, 'P0001', 'BR05');
SELECT pg_temp.expect_error('T22', 'BR04', 'Поїздка карткою 4000000000000018 після кінця строку дії (31.10.2026)',
    $q$CALL proc_register_trip('4000000000000018', 'V-30001', NULL, NULL, NULL, NULL, '2026-11-25 09:00')$q$, 'P0001', 'BR04');
SELECT pg_temp.expect_error('T23', 'BR16', 'Поповнення втраченої картки 4000000000000014',
    $q$CALL proc_top_up('4000000000000014', 100, 'online', 'TEST-020', NULL, NULL, '2026-11-25 09:00')$q$, 'P0001', 'BR16');

-- Тригер і відкат: зміни статусу та аудит скасовуються разом
DO $$
DECLARE v_hist_before integer; v_hist_inside integer; v_status text;
BEGIN
    SELECT count(*) INTO v_hist_before FROM card_status_history h JOIN cards c ON c.id = h.card_id
    WHERE c.card_number = '4999000000000002';
    BEGIN
        CALL proc_set_card_status('4999000000000002', 'blocked', 'Тест відкату');
        SELECT count(*) INTO v_hist_inside FROM card_status_history h JOIN cards c ON c.id = h.card_id
        WHERE c.card_number = '4999000000000002';
        RAISE EXCEPTION USING ERRCODE = 'P0099', MESSAGE = 'імітація збою після зміни статусу';
    EXCEPTION WHEN SQLSTATE 'P0099' THEN
        NULL;
    END;
    SELECT status INTO v_status FROM cards WHERE card_number = '4999000000000002';
    PERFORM pg_temp.t_log('T24', 'Тригер, ROLLBACK', 'Блокування картки, потім збій і відкат підтранзакції',
        'усередині: записів історії +1; після відкату: active, записів як до тесту',
        format('усередині %s → %s; після відкату %s, записів %s', v_hist_before, v_hist_inside, v_status,
               (SELECT count(*) FROM card_status_history h JOIN cards c ON c.id = h.card_id
                WHERE c.card_number = '4999000000000002')),
        v_hist_inside = v_hist_before + 1 AND v_status = 'active'
        AND (SELECT count(*) FROM card_status_history h JOIN cards c ON c.id = h.card_id
             WHERE c.card_number = '4999000000000002') = v_hist_before);
END $$;

-- Декларативні обмеження
SELECT pg_temp.expect_error('T25', 'BR06', 'Новий тариф на автобус з 01.12.2026 без закриття чинного',
    $q$INSERT INTO fares (transport_type_id, price, valid_from)
       SELECT id, 30, DATE '2026-12-01' FROM transport_types WHERE code = 'bus'$q$, '23P01');
SELECT pg_temp.expect_error('T26', 'BR08', 'Видалення картки 4000000000000001 з історією поїздок',
    $q$DELETE FROM cards WHERE card_number = '4000000000000001'$q$, '23001');
SELECT pg_temp.expect_error('T27', 'BR17', 'Видалення запису журналу списань власником схеми',
    $q$DELETE FROM charges WHERE id = (SELECT min(id) FROM charges)$q$, 'P0001', 'BR17');
SELECT pg_temp.expect_error('T28', 'BR13', 'Поїздка з методом subscription без посилання на проїзний',
    $q$INSERT INTO trips (card_id, validator_id, validated_at, payment_method)
       SELECT c.id, v.id, '2026-11-25 15:00', 'subscription'
       FROM cards c, validators v WHERE c.card_number = '4999000000000001' AND v.code = 'V-10001'$q$, '23514');

-- Права доступу (NFR01, NFR02): виконується від імені ролей
SELECT pg_temp.expect_error('T29', 'NFR01', 'Аналітик читає таблицю passengers',
    $q$SELECT * FROM passengers$q$, '42501', NULL, 'transit_analyst');
SELECT pg_temp.expect_error('T30', 'NFR02', 'Валідатор напряму змінює баланс картки',
    $q$UPDATE cards SET balance = balance + 100 WHERE card_number = '4999000000000001'$q$, '42501', NULL, 'transit_validator');
SELECT pg_temp.expect_error('T31', 'NFR02', 'Валідатор викликає процедуру поповнення',
    $q$CALL proc_top_up('4999000000000001', 100, 'online', 'TEST-030', NULL, NULL)$q$, '42501', NULL, 'transit_validator');

DO $$
DECLARE v_rows bigint;
BEGIN
    SET LOCAL ROLE transit_analyst;
    SELECT count(*) INTO v_rows FROM v_daily_revenue;
    RESET ROLE;
    PERFORM pg_temp.t_log('T32', 'NFR01', 'Аналітик читає представлення v_daily_revenue',
        'дозволено, рядків > 0', 'рядків ' || v_rows, v_rows > 0);
END $$;

-- Звірка кешованого балансу після всіх операцій тестів (FR05)
DO $$
DECLARE v_mismatch integer;
BEGIN
    SELECT count(*) INTO v_mismatch
    FROM cards AS c
    WHERE c.balance <> COALESCE((SELECT sum(amount) FROM top_ups WHERE card_id = c.id), 0)
                     - COALESCE((SELECT sum(amount) FROM charges WHERE card_id = c.id), 0);
    PERFORM pg_temp.t_log('T33', 'FR05', 'Звірка балансу всіх карток із журналами після тестів',
        '0 розбіжностей', v_mismatch || ' розбіжностей', v_mismatch = 0);
END $$;

-- Протокол
SELECT code, rule, action, expected, actual, CASE WHEN passed THEN 'так' ELSE 'НІ' END AS passed
FROM test_log ORDER BY code;

SELECT count(*) AS total, count(*) FILTER (WHERE passed) AS passed, count(*) FILTER (WHERE NOT passed) AS failed
FROM test_log;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM test_log WHERE NOT passed) THEN
        RAISE EXCEPTION 'Не пройдено тестів: %', (SELECT count(*) FROM test_log WHERE NOT passed);
    END IF;
END $$;

ROLLBACK;
