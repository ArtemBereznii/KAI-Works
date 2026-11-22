-- 03_functions.sql — функції та процедури.
--
-- Процедури, що змінюють гроші й стан карток, оголошено SECURITY DEFINER:
-- вони виконуються з правами власника схеми, тому ролям достатньо права EXECUTE,
-- а прямих прав на зміну таблиць вони не мають. SET search_path захищає такі
-- процедури від підміни об'єктів через search_path викликача.
--
-- Параметр p_at (момент операції, DEFAULT now()) стоїть останнім: валідатор і каса
-- передають фактичний момент, а наповнення тестових даних — історичний.
-- Повідомлення про порушення правил починаються з коду BRxx (docs/requirements.md).

SET search_path = transit, public;

-- ---------------------------------------------------------------------------
-- Допоміжні функції
-- ---------------------------------------------------------------------------

-- Хто виконує дію: роль після SET ROLE або користувач сеансу.
-- current_user не підходить — усередині SECURITY DEFINER це власник процедури.
CREATE FUNCTION fn_actor() RETURNS text
LANGUAGE sql STABLE
AS $$
    SELECT CASE WHEN current_setting('role') IN ('none', '')
                THEN session_user::text
                ELSE current_setting('role') END;
$$;
COMMENT ON FUNCTION fn_actor() IS 'Роль, від імені якої виконується дія (для аудиту)';

-- FR03: вартість разової поїздки для виду транспорту на момент часу.
-- Межі тарифів — календарні дати за київським часом. NULL, якщо тариф не визначено.
CREATE FUNCTION fn_current_fare(p_transport_type_id integer, p_at timestamptz DEFAULT now())
RETURNS numeric
LANGUAGE sql STABLE
AS $$
    SELECT f.price
    FROM transit.fares AS f
    WHERE f.transport_type_id = p_transport_type_id
      AND f.valid_from <= (p_at AT TIME ZONE 'Europe/Kyiv')::date
      AND (f.valid_to IS NULL OR f.valid_to > (p_at AT TIME ZONE 'Europe/Kyiv')::date);
$$;
COMMENT ON FUNCTION fn_current_fare(integer, timestamptz) IS 'Чинна ціна поїздки (BR06 гарантує не більше одного рядка)';

-- Знижка власника картки на момент часу: активна пільгова категорія,
-- строк дії пільги не минув. Анонімна картка або без пільги — 0.
CREATE FUNCTION fn_card_discount(p_card_id integer, p_at timestamptz DEFAULT now())
RETURNS numeric
LANGUAGE sql STABLE
AS $$
    SELECT COALESCE((
        SELECT pc.discount_pct
        FROM transit.cards AS c
        JOIN transit.passengers AS p ON p.id = c.passenger_id
        JOIN transit.privilege_categories AS pc ON pc.id = p.privilege_category_id
        WHERE c.id = p_card_id
          AND pc.is_active
          AND (p.privilege_valid_until IS NULL
               OR p.privilege_valid_until >= (p_at AT TIME ZONE 'Europe/Kyiv')::date)
    ), 0);
$$;
COMMENT ON FUNCTION fn_card_discount(integer, timestamptz) IS 'Відсоток пільгової знижки для картки на момент часу';

-- ---------------------------------------------------------------------------
-- FR02: виписка операцій картки за напіввідкритий інтервал [p_from, p_to)
-- ---------------------------------------------------------------------------
CREATE FUNCTION fn_card_statement(p_card_number text, p_from timestamptz, p_to timestamptz)
RETURNS TABLE (
    op_at          timestamptz,
    op_type        text,
    amount         numeric,     -- «+» поповнення, «−» списання
    balance_after  numeric,
    details        text
)
LANGUAGE sql STABLE SECURITY DEFINER
SET search_path = transit, public
AS $$
    SELECT t.created_at, 'top_up', t.amount, t.balance_after,
           'канал ' || t.channel || ', платіж ' || t.payment_ref
    FROM top_ups AS t
    JOIN cards AS c ON c.id = t.card_id
    WHERE c.card_number = p_card_number
      AND t.created_at >= p_from AND t.created_at < p_to
    UNION ALL
    SELECT ch.charged_at, ch.charge_type, -ch.amount, ch.balance_after,
           CASE ch.charge_type
               WHEN 'trip' THEN 'поїздка, валідатор ' || v.code
               ELSE 'проїзний ' || tp.code
           END
           || CASE WHEN ch.discount_pct > 0 THEN ', пільга ' || ch.discount_pct || '%' ELSE '' END
    FROM charges AS ch
    JOIN cards AS c ON c.id = ch.card_id
    LEFT JOIN trips AS tr ON tr.id = ch.trip_id
    LEFT JOIN validators AS v ON v.id = tr.validator_id
    LEFT JOIN card_subscriptions AS s ON s.id = ch.subscription_id
    LEFT JOIN tariff_plans AS tp ON tp.id = s.tariff_plan_id
    WHERE c.card_number = p_card_number
      AND ch.charged_at >= p_from AND ch.charged_at < p_to
    ORDER BY 1, 4 DESC;
$$;
COMMENT ON FUNCTION fn_card_statement(text, timestamptz, timestamptz) IS 'Виписка поповнень і списань картки за період';

-- ---------------------------------------------------------------------------
-- Видача картки
-- ---------------------------------------------------------------------------
CREATE PROCEDURE proc_issue_card(
    p_card_number   text,
    p_passenger_id  integer,               -- NULL — анонімна картка
    OUT p_card_id   integer,
    p_valid_until   date DEFAULT NULL,     -- за замовчуванням — 5 років від видачі
    p_at            timestamptz DEFAULT now()
)
LANGUAGE plpgsql SECURITY DEFINER
SET search_path = transit, public
AS $$
DECLARE
    v_issued date := (p_at AT TIME ZONE 'Europe/Kyiv')::date;
BEGIN
    PERFORM set_config('transit.status_reason', 'Видача картки', true);
    INSERT INTO cards (card_number, passenger_id, issued_on, valid_until, status_changed_at)
    VALUES (p_card_number, p_passenger_id, v_issued,
            COALESCE(p_valid_until, (v_issued + interval '5 years')::date), p_at)
    RETURNING id INTO p_card_id;
    PERFORM set_config('transit.status_reason', '', true);
END;
$$;

-- ---------------------------------------------------------------------------
-- Зміна статусу картки (перевірку переходу виконує BEFORE-тригер, BR05)
-- ---------------------------------------------------------------------------
CREATE PROCEDURE proc_set_card_status(
    p_card_number  text,
    p_new_status   text,
    p_reason       text DEFAULT NULL
)
LANGUAGE plpgsql SECURITY DEFINER
SET search_path = transit, public
AS $$
DECLARE
    v_rows integer;
BEGIN
    -- причину передаємо AFTER-тригеру аудиту через параметр транзакції
    PERFORM set_config('transit.status_reason', COALESCE(p_reason, ''), true);
    UPDATE cards SET status = p_new_status
    WHERE card_number = p_card_number AND status IS DISTINCT FROM p_new_status;
    GET DIAGNOSTICS v_rows = ROW_COUNT;
    PERFORM set_config('transit.status_reason', '', true);

    IF v_rows = 0 THEN
        IF NOT EXISTS (SELECT 1 FROM cards WHERE card_number = p_card_number) THEN
            RAISE EXCEPTION 'Картку % не знайдено', p_card_number USING ERRCODE = 'no_data_found';
        END IF;
        RAISE NOTICE 'Картка % уже має статус %, змін немає', p_card_number, p_new_status;
    END IF;
END;
$$;

-- ---------------------------------------------------------------------------
-- Поповнення балансу (BR03, BR09, BR11, BR16)
-- ---------------------------------------------------------------------------
CREATE PROCEDURE proc_top_up(
    p_card_number      text,
    p_amount           numeric,
    p_channel          text,
    p_payment_ref      text,
    OUT p_top_up_id    bigint,
    OUT p_balance      numeric,
    p_at               timestamptz DEFAULT now()
)
LANGUAGE plpgsql SECURITY DEFINER
SET search_path = transit, public
AS $$
DECLARE
    v_card      cards%ROWTYPE;
    v_existing  top_ups%ROWTYPE;
BEGIN
    -- BR11: блокування рядка картки серіалізує всі грошові операції з нею
    SELECT * INTO v_card FROM cards WHERE card_number = p_card_number FOR UPDATE;
    IF NOT FOUND THEN
        RAISE EXCEPTION 'Картку % не знайдено', p_card_number USING ERRCODE = 'no_data_found';
    END IF;
    IF v_card.status = 'lost' THEN
        RAISE EXCEPTION 'BR16: втрачену картку % поповнити не можна', p_card_number;
    END IF;

    -- BR09: повтор того самого платежу не створює другого поповнення
    INSERT INTO top_ups (card_id, amount, channel, payment_ref, balance_after, created_at)
    VALUES (v_card.id, p_amount, p_channel, p_payment_ref, v_card.balance + p_amount, p_at)
    ON CONFLICT (payment_ref) DO NOTHING
    RETURNING id INTO p_top_up_id;

    IF p_top_up_id IS NULL THEN
        SELECT * INTO v_existing FROM top_ups WHERE payment_ref = p_payment_ref;
        IF v_existing.card_id <> v_card.id OR v_existing.amount <> p_amount THEN
            RAISE EXCEPTION 'BR09: платіж % уже використано для іншої операції', p_payment_ref;
        END IF;
        RAISE NOTICE 'BR09: платіж % уже зараховано, повтор проігноровано', p_payment_ref;
        p_top_up_id := v_existing.id;
        p_balance   := v_card.balance;
        RETURN;
    END IF;

    p_balance := v_card.balance + p_amount;
    UPDATE cards SET balance = p_balance WHERE id = v_card.id;
END;
$$;

-- ---------------------------------------------------------------------------
-- Купівля проїзного (BR02, BR04, BR07, BR11)
-- ---------------------------------------------------------------------------
CREATE PROCEDURE proc_buy_plan(
    p_card_number           text,
    p_plan_code             text,
    OUT p_subscription_id   integer,
    OUT p_amount            numeric,
    OUT p_balance           numeric,
    p_at                    timestamptz DEFAULT now(),
    p_valid_from            timestamptz DEFAULT NULL   -- NULL — з моменту купівлі
)
LANGUAGE plpgsql SECURITY DEFINER
SET search_path = transit, public
AS $$
DECLARE
    v_card      cards%ROWTYPE;
    v_plan      tariff_plans%ROWTYPE;
    v_from      timestamptz := COALESCE(p_valid_from, p_at);
    v_discount  numeric;
BEGIN
    SELECT * INTO v_card FROM cards WHERE card_number = p_card_number FOR UPDATE;
    IF NOT FOUND THEN
        RAISE EXCEPTION 'Картку % не знайдено', p_card_number USING ERRCODE = 'no_data_found';
    END IF;
    IF v_card.status <> 'active' THEN
        RAISE EXCEPTION 'BR04: картка % має статус %', p_card_number, v_card.status;
    END IF;
    IF (v_from AT TIME ZONE 'Europe/Kyiv')::date > v_card.valid_until THEN
        RAISE EXCEPTION 'BR04: строк дії картки % минув %', p_card_number, v_card.valid_until;
    END IF;
    IF v_from < p_at THEN
        RAISE EXCEPTION 'Початок дії проїзного (%) не може бути раніше моменту купівлі', v_from;
    END IF;

    SELECT * INTO v_plan FROM tariff_plans WHERE code = p_plan_code;
    IF NOT FOUND OR NOT v_plan.is_active THEN
        RAISE EXCEPTION 'Тарифний план % не знайдено або він не продається', p_plan_code;
    END IF;

    v_discount := fn_card_discount(v_card.id, p_at);
    IF v_discount >= 100 THEN
        RAISE EXCEPTION 'Власник картки % їздить безкоштовно, проїзний не потрібен', p_card_number;
    END IF;
    p_amount := round(v_plan.price * (100 - v_discount) / 100, 2);
    IF v_card.balance < p_amount THEN
        RAISE EXCEPTION 'BR02: недостатньо коштів на картці % (баланс %, потрібно %)',
            p_card_number, v_card.balance, p_amount;
    END IF;

    -- Вичерпаний пакет поїздок, що ще формально діє, закривається з початком нового проїзного,
    -- інакше EXCLUDE (BR07) не дасть купити новий.
    UPDATE card_subscriptions
    SET valid_to = v_from
    WHERE card_id = v_card.id AND trips_left = 0
      AND valid_from < v_from AND valid_to > v_from;

    INSERT INTO card_subscriptions (card_id, tariff_plan_id, valid_from, valid_to, trips_left, purchased_at)
    VALUES (v_card.id, v_plan.id, v_from, v_from + make_interval(days => v_plan.duration_days),
            v_plan.trips_count, p_at)
    RETURNING id INTO p_subscription_id;

    p_balance := v_card.balance - p_amount;
    INSERT INTO charges (card_id, charge_type, subscription_id, full_price, discount_pct,
                         amount, balance_after, charged_at)
    VALUES (v_card.id, 'plan_purchase', p_subscription_id, v_plan.price, v_discount,
            p_amount, p_balance, p_at);
    UPDATE cards SET balance = p_balance WHERE id = v_card.id;
END;
$$;

-- ---------------------------------------------------------------------------
-- Реєстрація поїздки на валідаторі — центральна операція системи
-- Порядок оплати: проїзний → безкоштовна пересадка → 100 % пільга → баланс.
-- ---------------------------------------------------------------------------
CREATE PROCEDURE proc_register_trip(
    p_card_number         text,
    p_validator_code      text,
    OUT p_trip_id         bigint,
    OUT p_payment_method  text,
    OUT p_amount          numeric,
    OUT p_balance         numeric,
    p_at                  timestamptz DEFAULT now()
)
LANGUAGE plpgsql SECURITY DEFINER
SET search_path = transit, public
AS $$
DECLARE
    v_card       cards%ROWTYPE;
    v_validator  validators%ROWTYPE;
    v_sub_id     integer;
    v_sub_left   smallint;
    v_paid_trip  bigint;
    v_price      numeric;
    v_discount   numeric;
BEGIN
    SELECT * INTO v_validator FROM validators WHERE code = p_validator_code;
    IF NOT FOUND OR NOT v_validator.is_active THEN
        RAISE EXCEPTION 'Валідатор % не знайдено або він неактивний', p_validator_code;
    END IF;

    -- BR11: до кінця транзакції інші операції з цією карткою чекають
    SELECT * INTO v_card FROM cards WHERE card_number = p_card_number FOR UPDATE;
    IF NOT FOUND THEN
        RAISE EXCEPTION 'Картку % не знайдено', p_card_number USING ERRCODE = 'no_data_found';
    END IF;
    -- BR04
    IF v_card.status <> 'active' THEN
        RAISE EXCEPTION 'BR04: картка % має статус %, прохід заборонено', p_card_number, v_card.status;
    END IF;
    IF (p_at AT TIME ZONE 'Europe/Kyiv')::date > v_card.valid_until THEN
        RAISE EXCEPTION 'BR04: строк дії картки % минув %', p_card_number, v_card.valid_until;
    END IF;

    p_amount  := 0;
    p_balance := v_card.balance;

    -- 1. Проїзний, що діє на цей вид транспорту (BR07: такий проїзний не більше одного)
    SELECT s.id, s.trips_left INTO v_sub_id, v_sub_left
    FROM card_subscriptions AS s
    JOIN tariff_plans AS tp ON tp.id = s.tariff_plan_id
    WHERE s.card_id = v_card.id
      AND p_at >= s.valid_from AND p_at < s.valid_to
      AND (tp.transport_type_id IS NULL OR tp.transport_type_id = v_validator.transport_type_id)
      AND (s.trips_left IS NULL OR s.trips_left > 0);

    IF FOUND THEN
        p_payment_method := 'subscription';
        INSERT INTO trips (card_id, validator_id, validated_at, payment_method, subscription_id)
        VALUES (v_card.id, v_validator.id, p_at, p_payment_method, v_sub_id)
        RETURNING id INTO p_trip_id;
        IF v_sub_left IS NOT NULL THEN
            UPDATE card_subscriptions SET trips_left = trips_left - 1 WHERE id = v_sub_id;
        END IF;
        RETURN;
    END IF;

    -- 2. BR10: безкоштовна пересадка протягом 60 хв після платної поїздки
    --    на іншому валідаторі; одна пересадка на одну платну поїздку.
    SELECT t.id INTO v_paid_trip
    FROM trips AS t
    WHERE t.card_id = v_card.id
      AND t.payment_method = 'balance'
      AND t.validated_at >= p_at - interval '60 minutes'
      AND t.validated_at < p_at
      AND t.validator_id <> v_validator.id
      AND NOT EXISTS (SELECT 1 FROM trips AS x WHERE x.transfer_from_trip_id = t.id)
    ORDER BY t.validated_at DESC
    LIMIT 1;

    IF FOUND THEN
        p_payment_method := 'transfer';
        INSERT INTO trips (card_id, validator_id, validated_at, payment_method, transfer_from_trip_id)
        VALUES (v_card.id, v_validator.id, p_at, p_payment_method, v_paid_trip)
        RETURNING id INTO p_trip_id;
        RETURN;
    END IF;

    -- 3. Разова поїздка за тарифом із пільгою
    v_price := fn_current_fare(v_validator.transport_type_id, p_at);
    IF v_price IS NULL THEN
        RAISE EXCEPTION 'Тариф для валідатора % на % не визначено', p_validator_code, p_at;
    END IF;
    v_discount := fn_card_discount(v_card.id, p_at);

    IF v_discount >= 100 THEN
        p_payment_method := 'free';
        INSERT INTO trips (card_id, validator_id, validated_at, payment_method)
        VALUES (v_card.id, v_validator.id, p_at, p_payment_method)
        RETURNING id INTO p_trip_id;
        RETURN;
    END IF;

    p_amount := round(v_price * (100 - v_discount) / 100, 2);
    IF v_card.balance < p_amount THEN
        RAISE EXCEPTION 'BR02: недостатньо коштів на картці % (баланс %, потрібно %)',
            p_card_number, v_card.balance, p_amount;
    END IF;

    p_payment_method := 'balance';
    p_balance := v_card.balance - p_amount;
    INSERT INTO trips (card_id, validator_id, validated_at, payment_method)
    VALUES (v_card.id, v_validator.id, p_at, p_payment_method)
    RETURNING id INTO p_trip_id;
    INSERT INTO charges (card_id, charge_type, trip_id, full_price, discount_pct,
                         amount, balance_after, charged_at)
    VALUES (v_card.id, 'trip', p_trip_id, v_price, v_discount, p_amount, p_balance, p_at);
    UPDATE cards SET balance = p_balance WHERE id = v_card.id;
END;
$$;

-- ---------------------------------------------------------------------------
-- Права на виконання: спершу забираємо право PUBLIC, яке PostgreSQL дає
-- кожній новій функції, потім видаємо явно за ролями.
-- ---------------------------------------------------------------------------
REVOKE EXECUTE ON ALL ROUTINES IN SCHEMA transit FROM PUBLIC;

GRANT EXECUTE ON FUNCTION fn_current_fare(integer, timestamptz),
                          fn_card_statement(text, timestamptz, timestamptz)
    TO transit_cashier;
GRANT EXECUTE ON PROCEDURE proc_issue_card, proc_set_card_status, proc_top_up, proc_buy_plan
    TO transit_cashier;
GRANT EXECUTE ON PROCEDURE proc_register_trip TO transit_validator;
