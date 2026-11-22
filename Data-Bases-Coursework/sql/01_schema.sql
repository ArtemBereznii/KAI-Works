-- 01_schema.sql — структура бази даних ІС електронних проїзних документів.
-- 12 таблиць у схемі transit. Коди BRxx посилаються на бізнес-правила
-- з docs/requirements.md; там же — як кожне правило перевіряється.
--
-- Загальні рішення:
--   * сурогатні ключі GENERATED ALWAYS AS IDENTITY; природні ключі (номер картки,
--     код валідатора, payment_ref) — окремими UNIQUE;
--   * гроші — numeric(10,2), без округлень з плаваючою комою;
--   * моменти подій — timestamptz; календарні межі дії — date;
--   * для історичних даних FK мають ON DELETE RESTRICT: картку чи довідник,
--     на які посилається історія операцій, видалити не можна (BR08);
--   * стани і канали — text + CHECK (перелік короткий і стабільний,
--     а CHECK простіше розширити, ніж тип ENUM).

SET search_path = transit, public;

-- 1. Пільгові категорії пасажирів
CREATE TABLE privilege_categories (
    id            smallint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    code          text         NOT NULL UNIQUE
                               CHECK (code ~ '^[a-z_]{2,30}$'),
    name          text         NOT NULL UNIQUE CHECK (btrim(name) <> ''),
    discount_pct  numeric(5,2) NOT NULL
                               CHECK (discount_pct > 0 AND discount_pct <= 100),
    is_active     boolean      NOT NULL DEFAULT true
);
COMMENT ON TABLE privilege_categories IS 'Пільгові категорії та розмір знижки на проїзд';

-- 2. Пасажири — власники персоналізованих карток
CREATE TABLE passengers (
    id                      integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    last_name               text NOT NULL CHECK (btrim(last_name) <> ''),
    first_name              text NOT NULL CHECK (btrim(first_name) <> ''),
    middle_name             text,
    birth_date              date NOT NULL CHECK (birth_date >= DATE '1900-01-01'),
    phone                   text UNIQUE CHECK (phone ~ '^\+380[0-9]{9}$'),
    email                   text UNIQUE CHECK (email ~* '^[^@\s]+@[^@\s]+\.[a-z]{2,}$'),
    privilege_category_id   smallint
                            REFERENCES privilege_categories (id) ON DELETE RESTRICT,
    privilege_valid_until   date,
    registered_at           timestamptz NOT NULL DEFAULT now(),
    -- строк дії пільги має сенс лише разом із самою пільгою
    CONSTRAINT passengers_privilege_until_chk
        CHECK (privilege_valid_until IS NULL OR privilege_category_id IS NOT NULL)
);
COMMENT ON TABLE passengers IS 'Пасажири; картка може не мати власника (анонімна)';

-- 3. Транспортні картки
CREATE TABLE cards (
    id                 integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    card_number        text NOT NULL UNIQUE
                       CHECK (card_number ~ '^[0-9]{16}$'),                  -- BR01
    passenger_id       integer REFERENCES passengers (id) ON DELETE RESTRICT, -- NULL = анонімна
    status             text NOT NULL DEFAULT 'active'
                       CHECK (status IN ('active', 'blocked', 'lost')),
    -- Свідома денормалізація: кеш «сума поповнень − сума списань».
    -- Змінюється лише процедурами разом із записом у top_ups / charges.
    balance            numeric(10,2) NOT NULL DEFAULT 0
                       CHECK (balance >= 0),                                 -- BR02
    issued_on          date NOT NULL DEFAULT current_date,
    valid_until        date NOT NULL,
    status_changed_at  timestamptz NOT NULL DEFAULT now(),
    CONSTRAINT cards_validity_chk CHECK (valid_until > issued_on)
);
COMMENT ON TABLE cards IS 'Транспортні картки; прострочення визначається valid_until, а не статусом';
COMMENT ON COLUMN cards.balance IS 'Кешований баланс; звіряється з журналами top_ups і charges';

-- 4. Історія змін статусу картки (заповнює AFTER-тригер)
CREATE TABLE card_status_history (
    id          bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    card_id     integer NOT NULL REFERENCES cards (id) ON DELETE RESTRICT,
    old_status  text,                       -- NULL для першого запису (видача картки)
    new_status  text NOT NULL,
    changed_at  timestamptz NOT NULL DEFAULT clock_timestamp(),
    changed_by  text NOT NULL DEFAULT current_user,
    reason      text,
    CONSTRAINT card_status_history_change_chk
        CHECK (old_status IS DISTINCT FROM new_status)
);
COMMENT ON TABLE card_status_history IS 'Журнал аудиту статусів карток';

-- 5. Види транспорту
CREATE TABLE transport_types (
    id    smallint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    code  text NOT NULL UNIQUE CHECK (code ~ '^[a-z_]{2,20}$'),
    name  text NOT NULL UNIQUE CHECK (btrim(name) <> '')
);
COMMENT ON TABLE transport_types IS 'Довідник видів транспорту';

-- 6. Вартість разової поїздки з періодом дії
CREATE TABLE fares (
    id                 integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    transport_type_id  smallint NOT NULL
                       REFERENCES transport_types (id) ON DELETE RESTRICT,
    price              numeric(8,2) NOT NULL CHECK (price > 0),
    valid_from         date NOT NULL,
    valid_to           date,                -- NULL = діє безстроково; межа не включається
    CONSTRAINT fares_period_chk CHECK (valid_to IS NULL OR valid_to > valid_from),
    -- BR06: періоди дії тарифів одного виду транспорту не перетинаються
    CONSTRAINT fares_no_overlap_excl EXCLUDE USING gist (
        transport_type_id WITH =,
        daterange(valid_from, valid_to, '[)') WITH &&
    )
);
COMMENT ON TABLE fares IS 'Історія вартості разової поїздки за видом транспорту';

-- 7. Тарифні плани (проїзні)
CREATE TABLE tariff_plans (
    id                 smallint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    code               text NOT NULL UNIQUE CHECK (code ~ '^[A-Z0-9_]{2,30}$'),
    name               text NOT NULL CHECK (btrim(name) <> ''),
    plan_type          text NOT NULL CHECK (plan_type IN ('unlimited', 'trip_pack')),
    transport_type_id  smallint
                       REFERENCES transport_types (id) ON DELETE RESTRICT, -- NULL = усі види
    price              numeric(8,2) NOT NULL CHECK (price > 0),
    duration_days      smallint NOT NULL CHECK (duration_days BETWEEN 1 AND 366),
    trips_count        smallint CHECK (trips_count > 0),
    is_active          boolean NOT NULL DEFAULT true,
    -- кількість поїздок задається лише для пакета поїздок
    CONSTRAINT tariff_plans_type_chk CHECK (
        (plan_type = 'unlimited' AND trips_count IS NULL) OR
        (plan_type = 'trip_pack' AND trips_count IS NOT NULL)
    )
);
COMMENT ON TABLE tariff_plans IS 'Проїзні: безлімітний на період або пакет поїздок з терміном дії';

-- 8. Проїзні, придбані на картку
CREATE TABLE card_subscriptions (
    id              integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    card_id         integer NOT NULL REFERENCES cards (id) ON DELETE RESTRICT,
    tariff_plan_id  smallint NOT NULL REFERENCES tariff_plans (id) ON DELETE RESTRICT,
    valid_from      timestamptz NOT NULL,
    valid_to        timestamptz NOT NULL,   -- межа не включається
    trips_left      smallint CHECK (trips_left >= 0),  -- NULL для безлімітного
    purchased_at    timestamptz NOT NULL DEFAULT now(),
    CONSTRAINT card_subscriptions_period_chk CHECK (valid_to > valid_from),
    -- BR07: проїзні однієї картки не перетинаються в часі
    CONSTRAINT card_subscriptions_no_overlap_excl EXCLUDE USING gist (
        card_id WITH =,
        tstzrange(valid_from, valid_to, '[)') WITH &&
    )
);
COMMENT ON TABLE card_subscriptions IS
    'Проїзні на картці; вичерпаний пакет закривається моментом початку нового проїзного';

-- 9. Валідатори
CREATE TABLE validators (
    id                 integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    code               text NOT NULL UNIQUE CHECK (code ~ '^V-[0-9]{5}$'),
    transport_type_id  smallint NOT NULL
                       REFERENCES transport_types (id) ON DELETE RESTRICT,
    location           text NOT NULL CHECK (btrim(location) <> ''), -- маршрут, борт або станція
    is_active          boolean NOT NULL DEFAULT true,
    installed_on       date NOT NULL DEFAULT current_date
);
COMMENT ON TABLE validators IS 'Валідатори; вид транспорту поїздки визначається через валідатор';

-- 10. Поповнення балансу
CREATE TABLE top_ups (
    id             bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    card_id        integer NOT NULL REFERENCES cards (id) ON DELETE RESTRICT,
    amount         numeric(10,2) NOT NULL
                   CHECK (amount BETWEEN 1 AND 5000),                      -- BR03
    channel        text NOT NULL
                   CHECK (channel IN ('cash_desk', 'terminal', 'online', 'mobile_app')),
    payment_ref    text NOT NULL UNIQUE CHECK (btrim(payment_ref) <> ''),  -- BR09
    balance_after  numeric(10,2) NOT NULL CHECK (balance_after >= 0),
    created_at     timestamptz NOT NULL DEFAULT now()
);
COMMENT ON TABLE top_ups IS 'Журнал поповнень; payment_ref забезпечує ідемпотентність повтору платежу';

-- 11. Поїздки (валідації)
CREATE TABLE trips (
    id               bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    card_id          integer NOT NULL REFERENCES cards (id) ON DELETE RESTRICT,
    validator_id     integer NOT NULL REFERENCES validators (id) ON DELETE RESTRICT,
    validated_at     timestamptz NOT NULL,
    payment_method   text NOT NULL
                     CHECK (payment_method IN ('balance', 'subscription', 'transfer', 'free')),
    subscription_id  integer REFERENCES card_subscriptions (id) ON DELETE RESTRICT,
    -- BR10: платна поїздка, до якої прив'язана безкоштовна пересадка.
    -- UNIQUE гарантує не більше однієї пересадки на одну платну поїздку.
    transfer_from_trip_id bigint UNIQUE REFERENCES trips (id) ON DELETE RESTRICT,
    -- одна картка не може мати дві валідації в один і той самий момент
    CONSTRAINT trips_card_time_uq UNIQUE (card_id, validated_at),
    -- посилання на проїзний є тоді й лише тоді, коли поїздку оплачено проїзним
    CONSTRAINT trips_subscription_chk CHECK (
        (payment_method = 'subscription') = (subscription_id IS NOT NULL)
    ),
    -- посилання на платну поїздку є тоді й лише тоді, коли це пересадка
    CONSTRAINT trips_transfer_chk CHECK (
        (payment_method = 'transfer') = (transfer_from_trip_id IS NOT NULL)
    ),
    CONSTRAINT trips_transfer_not_self_chk CHECK (transfer_from_trip_id <> id)
);
COMMENT ON TABLE trips IS 'Поїздки: кожна успішна валідація картки';
COMMENT ON COLUMN trips.payment_method IS
    'balance — списання з балансу; subscription — проїзний; transfer — безкоштовна пересадка; free — 100% пільга';

-- 12. Списання коштів
CREATE TABLE charges (
    id               bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    card_id          integer NOT NULL REFERENCES cards (id) ON DELETE RESTRICT,
    charge_type      text NOT NULL CHECK (charge_type IN ('trip', 'plan_purchase')),
    trip_id          bigint  UNIQUE REFERENCES trips (id) ON DELETE RESTRICT,
    subscription_id  integer UNIQUE REFERENCES card_subscriptions (id) ON DELETE RESTRICT,
    full_price       numeric(10,2) NOT NULL CHECK (full_price > 0),
    -- застосована знижка фіксується на момент операції: пільга пасажира
    -- може змінитися пізніше, а історія списань — ні
    discount_pct     numeric(5,2) NOT NULL DEFAULT 0
                     CHECK (discount_pct >= 0 AND discount_pct < 100),
    amount           numeric(10,2) NOT NULL CHECK (amount > 0),
    balance_after    numeric(10,2) NOT NULL CHECK (balance_after >= 0),
    charged_at       timestamptz NOT NULL DEFAULT now(),
    CONSTRAINT charges_amount_chk CHECK (amount <= full_price),
    -- списання стосується або поїздки, або купівлі проїзного — рівно одного
    CONSTRAINT charges_target_chk CHECK (
        (charge_type = 'trip'          AND trip_id IS NOT NULL AND subscription_id IS NULL) OR
        (charge_type = 'plan_purchase' AND subscription_id IS NOT NULL AND trip_id IS NULL)
    )
);
COMMENT ON TABLE charges IS 'Журнал списань: оплата поїздки з балансу або купівля проїзного';

-- Індекси, яких немає серед індексів обмежень (PK/UNIQUE/EXCLUDE)
-- Виписка картки (fn_card_statement) і перевірка ON DELETE RESTRICT при видаленні картки.
CREATE INDEX top_ups_card_time_idx ON top_ups (card_id, created_at);
CREATE INDEX charges_card_time_idx ON charges (card_id, charged_at);
-- Індекси для trips за часом і за валідатором навмисно не створюються тут:
-- їхній вплив вимірюється в дослідженні продуктивності (09_perf.sql).
-- trips (card_id, validated_at) уже покрито обмеженням trips_card_time_uq.
