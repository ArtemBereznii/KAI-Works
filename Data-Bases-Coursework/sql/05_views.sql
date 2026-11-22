-- 05_views.sql — представлення.
-- Представлення виконуються з правами власника, тому аналітик читає дані
-- лише через них, не маючи доступу до базових таблиць і персональних даних.

SET search_path = transit, public;

-- ---------------------------------------------------------------------------
-- FR01: огляд картки для каси — власник, баланс, чинний проїзний, остання поїздка
-- ---------------------------------------------------------------------------
CREATE VIEW v_card_overview AS
WITH last_trip AS (
    -- DISTINCT ON: одна (остання) поїздка на картку; id — для однозначності порядку
    SELECT DISTINCT ON (t.card_id)
           t.card_id, t.validated_at, v.code AS validator_code, t.payment_method
    FROM trips AS t
    JOIN validators AS v ON v.id = t.validator_id
    ORDER BY t.card_id, t.validated_at DESC, t.id DESC
)
SELECT c.card_number,
       c.status,
       c.valid_until,
       c.valid_until < current_date                         AS is_expired,
       CASE WHEN p.id IS NULL THEN '(анонімна)'
            ELSE concat_ws(' ', p.last_name, p.first_name, p.middle_name) END AS owner_name,
       pc.name                                              AS privilege,
       c.balance,
       tp.code                                              AS active_plan,
       s.valid_to                                           AS plan_valid_to,
       s.trips_left                                         AS plan_trips_left,
       lt.validated_at                                      AS last_trip_at,
       lt.validator_code                                    AS last_validator,
       lt.payment_method                                    AS last_payment_method
FROM cards AS c
LEFT JOIN passengers AS p            ON p.id = c.passenger_id
LEFT JOIN privilege_categories AS pc ON pc.id = p.privilege_category_id
LEFT JOIN card_subscriptions AS s    ON s.card_id = c.id
                                    AND now() >= s.valid_from AND now() < s.valid_to
LEFT JOIN tariff_plans AS tp         ON tp.id = s.tariff_plan_id
LEFT JOIN last_trip AS lt            ON lt.card_id = c.id;

COMMENT ON VIEW v_card_overview IS 'Поточний стан картки для каси (BR07 гарантує не більше одного чинного проїзного)';

-- ---------------------------------------------------------------------------
-- Знеособлені факти поїздок для аналітики: без номерів карток і ПІБ
-- ---------------------------------------------------------------------------
CREATE VIEW v_trip_facts AS
SELECT t.id                                             AS trip_id,
       t.card_id,
       t.validated_at,
       (t.validated_at AT TIME ZONE 'Europe/Kyiv')::date AS trip_date,
       extract(hour FROM t.validated_at AT TIME ZONE 'Europe/Kyiv')::int AS trip_hour,
       tt.code                                          AS transport_type,
       v.code                                           AS validator_code,
       t.payment_method,
       COALESCE(ch.amount, 0)                           AS amount,
       COALESCE(ch.full_price, 0)                       AS full_price,
       (t.payment_method = 'free' OR ch.discount_pct > 0) AS is_privileged
FROM trips AS t
JOIN validators AS v       ON v.id = t.validator_id
JOIN transport_types AS tt ON tt.id = v.transport_type_id
LEFT JOIN charges AS ch    ON ch.trip_id = t.id;

COMMENT ON VIEW v_trip_facts IS 'Знеособлені поїздки (card_id — псевдонім, без номера картки)';

-- ---------------------------------------------------------------------------
-- FR04: добова виручка — поїздки vs продаж проїзних, поповнення
-- ---------------------------------------------------------------------------
CREATE VIEW v_daily_revenue AS
WITH ch AS (
    SELECT (charged_at AT TIME ZONE 'Europe/Kyiv')::date AS day,
           count(*) FILTER (WHERE charge_type = 'trip')            AS paid_trips,
           COALESCE(sum(amount) FILTER (WHERE charge_type = 'trip'), 0)          AS trip_revenue,
           count(*) FILTER (WHERE charge_type = 'plan_purchase')   AS plans_sold,
           COALESCE(sum(amount) FILTER (WHERE charge_type = 'plan_purchase'), 0) AS plan_revenue
    FROM charges
    GROUP BY 1
),
tr AS (
    SELECT (validated_at AT TIME ZONE 'Europe/Kyiv')::date AS day, count(*) AS all_trips
    FROM trips
    GROUP BY 1
),
tu AS (
    SELECT (created_at AT TIME ZONE 'Europe/Kyiv')::date AS day, sum(amount) AS top_ups_total
    FROM top_ups
    GROUP BY 1
)
SELECT d.day,
       COALESCE(tr.all_trips, 0)       AS all_trips,
       COALESCE(ch.paid_trips, 0)      AS paid_trips,
       COALESCE(ch.trip_revenue, 0)    AS trip_revenue,
       COALESCE(ch.plans_sold, 0)      AS plans_sold,
       COALESCE(ch.plan_revenue, 0)    AS plan_revenue,
       COALESCE(ch.trip_revenue, 0) + COALESCE(ch.plan_revenue, 0) AS total_revenue,
       COALESCE(tu.top_ups_total, 0)   AS top_ups_total
FROM (SELECT day FROM ch UNION SELECT day FROM tr UNION SELECT day FROM tu) AS d
LEFT JOIN ch USING (day)
LEFT JOIN tr USING (day)
LEFT JOIN tu USING (day);

COMMENT ON VIEW v_daily_revenue IS 'Виручка й кількість поїздок за днями (київський час)';

GRANT SELECT ON v_card_overview TO transit_cashier;
GRANT SELECT ON v_trip_facts, v_daily_revenue TO transit_analyst;
