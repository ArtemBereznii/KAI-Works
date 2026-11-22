\set ON_ERROR_STOP off
\set ON_ERROR_ROLLBACK on
\set VERBOSITY default
\set SHOW_CONTEXT never
\pset null '—'
\pset footer off
SET search_path = transit, public;
SET timezone = 'Europe/Kyiv';

\if :{?nopause}
\else
\set nopause 0
\endif

BEGIN;

\echo
\echo '════ 1. Структура: 12 таблиць і зв''язки ════'
\dt transit.*
SELECT conrelid::regclass AS "таблиця", confrelid::regclass AS "посилається на",
       pg_get_constraintdef(oid) AS "зовнішній ключ"
FROM pg_constraint
WHERE contype = 'f' AND connamespace = 'transit'::regnamespace
  AND conrelid::regclass::text IN ('cards', 'trips', 'charges')
ORDER BY 1, 2;
\if :nopause \else \prompt 'Enter — далі ' _ \endif

\echo
\echo '════ 2. Операція користувача: каса видає картку й поповнює, валідатор реєструє поїздки ════'
SET LOCAL ROLE transit_cashier;
CALL proc_issue_card('4555000000000001', NULL, NULL);
CALL proc_top_up('4555000000000001', 100, 'cash_desk', 'DEMO-001', NULL, NULL);
-- повтор того самого платежу (BR09): другого поповнення немає
CALL proc_top_up('4555000000000001', 100, 'cash_desk', 'DEMO-001', NULL, NULL);
RESET ROLE;
SET LOCAL ROLE transit_validator;
-- автобус: списання з балансу
CALL proc_register_trip('4555000000000001', 'V-10001', NULL, NULL, NULL, NULL, now());
-- метро через 30 хв: безкоштовна пересадка (BR10)
CALL proc_register_trip('4555000000000001', 'V-40001', NULL, NULL, NULL, NULL, now() + interval '30 minutes');
-- той самий валідатор ще раз: платно (оплата за супутника)
CALL proc_register_trip('4555000000000001', 'V-40001', NULL, NULL, NULL, NULL, now() + interval '40 minutes');
RESET ROLE;
SET LOCAL ROLE transit_cashier;
SELECT card_number, status, balance, last_payment_method, last_validator
FROM v_card_overview WHERE card_number = '4555000000000001';
SELECT op_type, amount, balance_after, details
FROM fn_card_statement('4555000000000001', now() - interval '1 day', now() + interval '1 day');
RESET ROLE;
\if :nopause \else \prompt 'Enter — далі ' _ \endif

\echo
\echo '════ 3. Аналітичний запит від імені аналітика (лише знеособлене представлення) ════'
\echo 'Зміна на місці: змінні transport (bus, trolleybus, tram, metro) і min_trips — див. docs/demo.md'
\set transport 'metro'
\set min_trips 20
SET LOCAL ROLE transit_analyst;
SELECT trip_hour AS "година",
       count(*) AS "поїздок",
       count(*) FILTER (WHERE payment_method = 'transfer') AS "пересадок",
       sum(amount) AS "виручка"
FROM v_trip_facts
WHERE transport_type = :'transport'
GROUP BY trip_hour
HAVING count(*) >= :min_trips
ORDER BY "поїздок" DESC;
RESET ROLE;
\if :nopause \else \prompt 'Enter — далі ' _ \endif

\echo
\echo '════ 4. Відмова при порушенні правил ════'
\echo '--- BR03: поповнення на 5000.01 грн'
CALL proc_top_up('4555000000000001', 5000.01, 'online', 'DEMO-002', NULL, NULL);
\echo '--- BR07: другий проїзний на той самий час (спершу купуємо тижневий — успішно)'
SET LOCAL ROLE transit_cashier;
CALL proc_top_up('4555000000000001', 1000, 'online', 'DEMO-003', NULL, NULL);
CALL proc_buy_plan('4555000000000001', 'W7_ALL', NULL, NULL, NULL);
CALL proc_buy_plan('4555000000000001', 'P10_ALL', NULL, NULL, NULL);
RESET ROLE;
\echo '--- BR05: втрачену картку не можна повернути в обіг'
CALL proc_set_card_status('4000000000000014', 'active', 'спроба');
\echo '--- BR08: картку з історією не можна видалити'
DELETE FROM cards WHERE card_number = '4000000000000001';
\echo '--- NFR01: аналітик не бачить персональних даних'
SET LOCAL ROLE transit_analyst;
SELECT last_name, phone FROM passengers LIMIT 1;
RESET ROLE;
\if :nopause \else \prompt 'Enter — далі ' _ \endif

\echo
\echo '════ 5. Тригер: зміна статусу з перевіркою переходу й аудитом ════'
SET LOCAL ROLE transit_cashier;
CALL proc_set_card_status('4555000000000001', 'blocked', 'Демонстрація на захисті');
RESET ROLE;
SELECT old_status, new_status, changed_by, reason
FROM card_status_history h JOIN cards c ON c.id = h.card_id
WHERE c.card_number = '4555000000000001' ORDER BY h.id;
\echo '--- поїздка заблокованою карткою (BR04)'
SET LOCAL ROLE transit_validator;
CALL proc_register_trip('4555000000000001', 'V-10001', NULL, NULL, NULL, NULL, now() + interval '2 hours');
RESET ROLE;
\if :nopause \else \prompt 'Enter — далі ' _ \endif

\echo
\echo '════ 6. Транзакція й швидкодія ════'
\echo '--- Атомарність: поїздка без коштів не залишає ні поїздки, ні списання'
SET LOCAL ROLE transit_cashier;
CALL proc_issue_card('4555000000000002', NULL, NULL);
RESET ROLE;
SET LOCAL ROLE transit_validator;
CALL proc_register_trip('4555000000000002', 'V-10001', NULL, NULL, NULL, NULL);
RESET ROLE;
SELECT count(*) AS "поїздок картки …0002", (SELECT balance FROM cards WHERE card_number = '4555000000000002') AS "баланс"
FROM trips t JOIN cards c ON c.id = t.card_id WHERE c.card_number = '4555000000000002';
\echo '--- Звірка кешованого балансу з журналами (має бути 0)'
SELECT count(*) AS "розбіжностей"
FROM cards AS c
WHERE c.balance <> COALESCE((SELECT sum(amount) FROM top_ups WHERE card_id = c.id), 0)
                 - COALESCE((SELECT sum(amount) FROM charges WHERE card_id = c.id), 0);
\echo '--- План запиту «останні 10 поїздок картки»: індекс UNIQUE (card_id, validated_at)'
EXPLAIN (ANALYZE, BUFFERS, COSTS OFF)
SELECT validated_at, payment_method FROM trips
WHERE card_id = (SELECT id FROM cards WHERE card_number = '4000000000000009')
ORDER BY validated_at DESC LIMIT 10;
\echo
\echo 'Відновлення з дампу показується окремою командою: ./restore_test.sh'

ROLLBACK;
\echo '════ Кінець: усі зміни демонстрації скасовано (ROLLBACK) ════'
