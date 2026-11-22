-- 06_seed.sql — тестові дані.
--
-- Довідники та пасажири вставляються напряму (це робота адміністратора).
-- Усі операції з картками — видача, поповнення, купівля проїзних, поїздки, зміни статусу —
-- виконуються ЛИШЕ процедурами з історичним моментом p_at. Тому журнали top_ups/charges,
-- кешований баланс і аудит статусів узгоджені так само, як у реальній роботі системи.
--
-- Період операцій: 2026-10-01 … 2026-11-21. 1 листопада змінюються тарифи — це дає
-- дані для запитів про історію цін. Поїздки генеруються псевдовипадково з фіксованим
-- setseed, тому набір відтворюється однаково при кожному прогоні.

SET search_path = transit, public;
SET timezone = 'Europe/Kyiv';

-- ---------------------------------------------------------------------------
-- Довідники
-- ---------------------------------------------------------------------------
INSERT INTO privilege_categories (code, name, discount_pct, is_active) VALUES
    ('student',      'Студент',                   50,  true),
    ('schoolchild',  'Учень',                     50,  true),
    ('pensioner',    'Пенсіонер',                 100, true),
    ('large_family', 'Член багатодітної родини',  25,  true),
    ('donor',        'Почесний донор',            20,  false);  -- пільгу скасовано

INSERT INTO transport_types (code, name) VALUES
    ('bus', 'Автобус'), ('trolleybus', 'Тролейбус'), ('tram', 'Трамвай'), ('metro', 'Метро');

-- Тарифи: до 1 листопада і з 1 листопада (безстроково)
INSERT INTO fares (transport_type_id, price, valid_from, valid_to)
SELECT tt.id, v.price, v.valid_from::date, v.valid_to::date
FROM (VALUES
    ('bus',        15, '2026-01-01', '2026-11-01'), ('bus',        20, '2026-11-01', NULL),
    ('trolleybus', 15, '2026-01-01', '2026-11-01'), ('trolleybus', 20, '2026-11-01', NULL),
    ('tram',       15, '2026-01-01', '2026-11-01'), ('tram',       20, '2026-11-01', NULL),
    ('metro',      20, '2026-01-01', '2026-11-01'), ('metro',      25, '2026-11-01', NULL)
) AS v (tt_code, price, valid_from, valid_to)
JOIN transport_types AS tt ON tt.code = v.tt_code;

INSERT INTO tariff_plans (code, name, plan_type, transport_type_id, price, duration_days, trips_count, is_active)
SELECT v.code, v.name, v.plan_type, tt.id, v.price, v.days, v.trips, v.active
FROM (VALUES
    ('M30_ALL',   'Місячний, усі види транспорту', 'unlimited', NULL,    1200, 30, NULL, true),
    ('M30_METRO', 'Місячний, метро',               'unlimited', 'metro',  800, 30, NULL, true),
    ('W7_ALL',    'Тижневий, усі види транспорту', 'unlimited', NULL,     350,  7, NULL, true),
    ('P10_ALL',   '10 поїздок, усі види',          'trip_pack', NULL,     170, 30, 10,   true),
    ('P20_BUS',   '20 поїздок, автобус',           'trip_pack', 'bus',    280, 30, 20,   true),
    ('M30_OLD',   'Місячний (архівний)',           'unlimited', NULL,     900, 30, NULL, false)
) AS v (code, name, plan_type, tt_code, price, days, trips, active)
LEFT JOIN transport_types AS tt ON tt.code = v.tt_code
ORDER BY v.code;

INSERT INTO validators (code, transport_type_id, location, is_active, installed_on)
SELECT v.code, tt.id, v.location, v.active, v.installed::date
FROM (VALUES
    ('V-10001', 'bus',        'Автобус №5, борт 1105',            true,  '2024-03-01'),
    ('V-10002', 'bus',        'Автобус №23, борт 2311',           true,  '2024-03-01'),
    ('V-10003', 'bus',        'Автобус №47, борт 4702',           true,  '2025-01-15'),
    ('V-10004', 'bus',        'Автобус №71, борт 7120',           true,  '2025-01-15'),
    ('V-20001', 'trolleybus', 'Тролейбус №2, борт 0207',          true,  '2024-05-10'),
    ('V-20002', 'trolleybus', 'Тролейбус №9, борт 0914',          true,  '2024-05-10'),
    ('V-30001', 'tram',       'Трамвай №1, вагон 101',            true,  '2024-06-01'),
    ('V-30002', 'tram',       'Трамвай №8, вагон 812',            true,  '2024-06-01'),
    ('V-40001', 'metro',      'Станція «Центральна», вхід 1',     true,  '2023-09-01'),
    ('V-40002', 'metro',      'Станція «Університет», вхід 2',    true,  '2023-09-01'),
    ('V-40003', 'metro',      'Станція «Вокзальна», вхід 1',      true,  '2023-09-01'),
    ('V-40004', 'metro',      'Станція «Річкова», вхід 1 (ремонт)', false, '2023-09-01')
) AS v (code, tt_code, location, active, installed)
JOIN transport_types AS tt ON tt.code = v.tt_code
ORDER BY v.code;

-- Синтетичні пасажири (будь-який збіг з реальними особами випадковий)
INSERT INTO passengers (last_name, first_name, middle_name, birth_date, phone, email,
                        privilege_category_id, privilege_valid_until, registered_at)
SELECT v.ln, v.fn, v.mn, v.bd::date, v.phone, v.email, pc.id, v.priv_until::date, v.reg::timestamptz
FROM (VALUES
    ('Коваленко',  'Олена',   'Петрівна',     '2005-04-12', '+380501110001', 'o.kovalenko@example.com', 'student',      '2027-06-30', '2025-09-01 10:00'),
    ('Шевченко',   'Андрій',  'Іванович',     '1988-11-03', '+380501110002', 'a.shevchenko@example.com', NULL,          NULL,         '2025-02-14 12:30'),
    ('Бондаренко', 'Марія',   'Степанівна',   '1951-07-21', '+380501110003', NULL,                       'pensioner',   NULL,         '2024-11-20 09:15'),
    ('Ткаченко',   'Ігор',    'Миколайович',  '1979-01-30', '+380501110004', 'i.tkachenko@example.com',  NULL,          NULL,         '2025-06-02 17:40'),
    ('Кравченко',  'Софія',   'Андріївна',    '2011-09-09', NULL,            NULL,                       'schoolchild', '2027-05-31', '2025-08-28 11:00'),
    ('Олійник',    'Дмитро',  'Олегович',     '1993-03-15', '+380501110006', 'd.oliinyk@example.com',    NULL,          NULL,         '2025-03-10 08:20'),
    ('Мельник',    'Ганна',   'Василівна',    '1949-12-01', '+380501110007', NULL,                       'pensioner',   NULL,         '2024-10-05 13:05'),
    ('Лисенко',    'Тарас',   'Романович',    '2003-05-27', '+380501110008', 't.lysenko@example.com',    'student',     '2026-10-15', '2024-09-02 09:00'),
    ('Руденко',    'Ірина',   'Сергіївна',    '1985-08-08', '+380501110009', 'i.rudenko@example.com',    'large_family', NULL,        '2025-01-20 15:45'),
    ('Савченко',   'Павло',   'Юрійович',     '1990-02-17', '+380501110010', 'p.savchenko@example.com',  'donor',       NULL,         '2025-04-11 10:10'),
    ('Мороз',      'Наталія', 'Ігорівна',     '1996-10-22', '+380501110011', 'n.moroz@example.com',      NULL,          NULL,         '2025-07-07 16:00'),
    ('Гончаренко', 'Віктор',  'Петрович',     '1975-06-05', '+380501110012', NULL,                       NULL,          NULL,         '2025-05-19 12:00'),
    ('Петренко',   'Юлія',    'Олександрівна','1999-12-31', '+380501110013', 'y.petrenko@example.com',   NULL,          NULL,         '2025-10-01 14:30'),
    ('Захарченко', 'Олег',    NULL,           '1983-04-04', '+380501110014', 'o.zakharchenko@example.com', NULL,        NULL,         '2026-09-25 18:00')
) AS v (ln, fn, mn, bd, phone, email, priv, priv_until, reg)
LEFT JOIN privilege_categories AS pc ON pc.code = v.priv
ORDER BY v.reg;   -- id призначаються в порядку реєстрації


-- ---------------------------------------------------------------------------
-- Картки та профілі поведінки (профіль потрібен лише генератору нижче)
--   daily      — поїздки майже щодня (будні 90 %, вихідні 35 %), туди й назад
--   occasional — 30 % днів
--   none       — картка є, але не використовується
-- plan_code — проїзний, який власник купує знову, щойно попередній закінчився.
-- ---------------------------------------------------------------------------
CREATE TEMP TABLE seed_cards (
    card_number  text PRIMARY KEY,
    owner_last   text,        -- прізвище власника; NULL — анонімна картка
    issued_at    timestamptz,
    valid_until  date,        -- NULL — стандартні 5 років
    profile      text,
    plan_code    text,
    homes        text[]       -- валідатори, якими користується власник
);

INSERT INTO seed_cards VALUES
    ('4000000000000001', 'Коваленко',  '2025-09-01 10:05', NULL,         'daily',      NULL,        '{V-40001,V-10001,V-20001}'),
    ('4000000000000002', 'Шевченко',   '2025-02-14 12:35', NULL,         'daily',      'M30_ALL',   '{V-40002,V-30001,V-10002}'),
    ('4000000000000003', 'Бондаренко', '2024-11-20 09:20', NULL,         'occasional', NULL,        '{V-10003,V-30002}'),
    ('4000000000000004', 'Ткаченко',   '2025-06-02 17:45', NULL,         'daily',      'M30_METRO', '{V-40001,V-40003,V-10002}'),
    ('4000000000000005', 'Кравченко',  '2025-08-28 11:05', NULL,         'daily',      NULL,        '{V-30001,V-20001}'),
    ('4000000000000006', 'Олійник',    '2025-03-10 08:25', NULL,         'daily',      'P10_ALL',   '{V-40003,V-20002}'),
    ('4000000000000007', 'Олійник',    '2026-06-15 19:00', NULL,         'occasional', NULL,        '{V-10004,V-40002}'),
    ('4000000000000008', 'Мельник',    '2024-10-05 13:10', NULL,         'occasional', NULL,        '{V-20001,V-10001}'),
    ('4000000000000009', 'Лисенко',    '2024-09-02 09:05', NULL,         'daily',      NULL,        '{V-40002,V-10003}'),
    ('4000000000000010', 'Руденко',    '2025-01-20 15:50', NULL,         'daily',      NULL,        '{V-30002,V-10004,V-20002}'),
    ('4000000000000011', 'Савченко',   '2025-04-11 10:15', NULL,         'occasional', NULL,        '{V-40001,V-10001}'),
    ('4000000000000012', 'Мороз',      '2025-07-07 16:05', NULL,         'daily',      'W7_ALL',    '{V-40003,V-30001}'),
    ('4000000000000013', 'Гончаренко', '2025-05-19 12:05', NULL,         'daily',      NULL,        '{V-10002,V-20002}'),
    ('4000000000000014', 'Петренко',   '2025-10-01 14:35', NULL,         'occasional', NULL,        '{V-40002,V-30002}'),
    ('4000000000000015', NULL,         '2025-12-03 08:00', NULL,         'occasional', NULL,        '{V-10001,V-40003}'),
    ('4000000000000016', NULL,         '2026-02-10 07:30', NULL,         'daily',      'P20_BUS',   '{V-10001,V-10003,V-10004}'),
    ('4000000000000017', NULL,         '2026-09-30 18:20', NULL,         'none',       NULL,        '{}'),
    ('4000000000000018', NULL,         '2021-11-01 09:00', '2026-10-31', 'occasional', NULL,        '{V-30001,V-40001}');

-- Видача карток — через процедуру (AFTER-тригер записує видачу в історію статусів)
DO $$
DECLARE
    r     record;
    v_id  integer;
BEGIN
    FOR r IN
        SELECT sc.*, p.id AS passenger_id
        FROM seed_cards AS sc
        LEFT JOIN transit.passengers AS p ON p.last_name = sc.owner_last
        ORDER BY sc.issued_at
    LOOP
        CALL transit.proc_issue_card(r.card_number, r.passenger_id, v_id, r.valid_until, r.issued_at);
    END LOOP;
END
$$;

-- Картка 17: одне поповнення й жодної поїздки (для запитів про невикористані картки)
CALL proc_top_up('4000000000000017', 100, 'cash_desk', 'SEED-INIT-17', NULL, NULL, '2026-09-30 18:25');

-- ---------------------------------------------------------------------------
-- Генератор операцій за 2026-10-01 … 2026-11-21
-- ---------------------------------------------------------------------------
DO $$
DECLARE
    c            record;
    d            date;
    leg          integer;
    v_prob       double precision;
    v_t          timestamptz;
    v_val        text;
    v_val2       text;
    v_ref_no     integer := 0;
    v_ok         integer := 0;
    v_failed     integer := 0;
    v_balance    numeric;
    v_price      numeric;
    v_discount   numeric;
    -- OUT-параметри процедур
    o_id bigint; o_id2 integer; o_method text; o_amount numeric; o_balance numeric;
    v_channels   text[] := ARRAY['cash_desk', 'terminal', 'online', 'mobile_app'];
BEGIN
    PERFORM setseed(0.67);   -- відтворюваність набору

    FOR d IN SELECT generate_series(DATE '2026-10-01', DATE '2026-11-21', interval '1 day')::date LOOP
        FOR c IN
            SELECT sc.*, ca.id AS card_id, ca.valid_until AS card_valid_until
            FROM seed_cards AS sc
            JOIN transit.cards AS ca USING (card_number)
            WHERE sc.profile <> 'none'
            ORDER BY sc.card_number
        LOOP
            CONTINUE WHEN d > c.card_valid_until;

            v_prob := CASE c.profile
                          WHEN 'daily' THEN CASE WHEN extract(isodow FROM d) >= 6 THEN 0.35 ELSE 0.9 END
                          ELSE 0.3 END;
            CONTINUE WHEN random() >= v_prob;

            FOR leg IN 1..2 LOOP     -- 1 — ранкова поїздка, 2 — вечірня
                v_t := ((d + CASE leg WHEN 1 THEN time '07:00' ELSE time '17:00' END)
                        AT TIME ZONE 'Europe/Kyiv')
                       + random() * interval '150 minutes';
                v_val := c.homes[1 + floor(random() * array_length(c.homes, 1))::int];
                v_discount := transit.fn_card_discount(c.card_id, v_t);

                -- проїзний закінчився або вичерпаний — купуємо новий
                IF c.plan_code IS NOT NULL AND NOT EXISTS (
                        SELECT 1 FROM transit.card_subscriptions AS s
                        WHERE s.card_id = c.card_id
                          AND v_t - interval '10 minutes' >= s.valid_from
                          AND v_t - interval '10 minutes' <  s.valid_to
                          AND (s.trips_left IS NULL OR s.trips_left > 0)) THEN
                    SELECT round(price * (100 - v_discount) / 100, 2) INTO v_price
                    FROM transit.tariff_plans WHERE code = c.plan_code;
                    SELECT balance INTO v_balance FROM transit.cards WHERE id = c.card_id;
                    IF v_balance < v_price THEN
                        v_ref_no := v_ref_no + 1;
                        CALL transit.proc_top_up(c.card_number, ceil((v_price - v_balance + 100) / 100) * 100,
                                                 'online', 'SEED-' || lpad(v_ref_no::text, 5, '0'),
                                                 o_id, o_balance, v_t - interval '30 minutes');
                    END IF;
                    CALL transit.proc_buy_plan(c.card_number, c.plan_code, o_id2, o_amount, o_balance,
                                               v_t - interval '10 minutes');
                END IF;

                -- поповнення, коли коштів замало (пасажирам зі 100 % пільгою не потрібне)
                SELECT balance INTO v_balance FROM transit.cards WHERE id = c.card_id;
                IF v_discount < 100 AND v_balance < 60 THEN
                    v_ref_no := v_ref_no + 1;
                    CALL transit.proc_top_up(c.card_number,
                                             (ARRAY[200, 300, 500])[1 + floor(random() * 3)::int],
                                             v_channels[1 + floor(random() * 4)::int],
                                             'SEED-' || lpad(v_ref_no::text, 5, '0'),
                                             o_id, o_balance, v_t - interval '20 minutes');
                END IF;

                BEGIN
                    CALL transit.proc_register_trip(c.card_number, v_val, o_id, o_method, o_amount, o_balance, v_t);
                    v_ok := v_ok + 1;

                    -- пересадка на інший валідатор через 8–38 хв (35 % поїздок)
                    IF array_length(c.homes, 1) > 1 AND random() < 0.35 THEN
                        v_val2 := (SELECT h FROM unnest(c.homes) AS h WHERE h <> v_val
                                   ORDER BY h OFFSET floor(random() * (array_length(c.homes, 1) - 1))::int LIMIT 1);
                        CALL transit.proc_register_trip(c.card_number, v_val2, o_id, o_method, o_amount, o_balance,
                                                        v_t + interval '8 minutes' + random() * interval '30 minutes');
                        v_ok := v_ok + 1;
                    END IF;
                EXCEPTION WHEN OTHERS THEN
                    v_failed := v_failed + 1;
                    RAISE NOTICE 'Пропущено поїздку % о %: %', c.card_number, v_t, SQLERRM;
                END;
            END LOOP;
        END LOOP;
    END LOOP;

    RAISE NOTICE 'Згенеровано поїздок: %, відхилено: %', v_ok, v_failed;
END
$$;

-- ---------------------------------------------------------------------------
-- Зміни статусів (через процедуру; аудит фіксує фактичний момент виконання скрипта)
-- ---------------------------------------------------------------------------
CALL proc_set_card_status('4000000000000013', 'blocked', 'Підозра на шахрайство: звернення банку');
CALL proc_set_card_status('4000000000000014', 'blocked', 'Заява власника про втрату');
CALL proc_set_card_status('4000000000000014', 'lost',    'Картку не знайдено протягом 3 днів');
CALL proc_set_card_status('4000000000000011', 'blocked', 'Перевірка пільгових документів');
CALL proc_set_card_status('4000000000000011', 'active',  'Перевірку завершено');

DROP TABLE seed_cards;

ANALYZE;

-- Підсумок наповнення
SELECT 'passengers' AS table_name, count(*) FROM passengers
UNION ALL SELECT 'cards',               count(*) FROM cards
UNION ALL SELECT 'card_status_history', count(*) FROM card_status_history
UNION ALL SELECT 'card_subscriptions',  count(*) FROM card_subscriptions
UNION ALL SELECT 'top_ups',             count(*) FROM top_ups
UNION ALL SELECT 'trips',               count(*) FROM trips
UNION ALL SELECT 'charges',             count(*) FROM charges;
