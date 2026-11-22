-- 07_queries.sql — інформаційні запити (Q01–Q18) та операції зміни даних (M1–M4).
-- Кожен запит починається з питання користувача. Запити розраховано на дані з 06_seed.sql;
-- «поточний момент» звітів зафіксовано змінною report_at, щоб результат не залежав від дати запуску.
-- Операції зміни виконуються в кінці, після всіх SELECT.

SET search_path = transit, public;
SET timezone = 'Europe/Kyiv';
\set report_at '''2026-11-22 00:00:00+02'''
\pset null '—'

\echo '=== Q01. Скільки поїздок і яку виручку дав кожен вид транспорту в листопаді? (JOIN, GROUP BY, FILTER)'
SELECT tt.name                                                   AS transport,
       count(t.id)                                               AS trips,
       count(t.id) FILTER (WHERE t.payment_method = 'balance')   AS paid_trips,
       COALESCE(sum(ch.amount), 0)                               AS revenue
FROM transport_types AS tt
JOIN validators AS v ON v.transport_type_id = tt.id
LEFT JOIN trips AS t ON t.validator_id = v.id
                    AND t.validated_at >= '2026-11-01' AND t.validated_at < '2026-12-01'
LEFT JOIN charges AS ch ON ch.trip_id = t.id
GROUP BY tt.id, tt.name
ORDER BY trips DESC;

\echo '=== Q02. Які картки видано, але жодного разу не використано? (NOT EXISTS)'
SELECT c.card_number, c.issued_on, c.balance,
       COALESCE(p.last_name || ' ' || p.first_name, '(анонімна)') AS owner
FROM cards AS c
LEFT JOIN passengers AS p ON p.id = c.passenger_id
WHERE NOT EXISTS (SELECT 1 FROM trips AS t WHERE t.card_id = c.id)
ORDER BY c.card_number;

\echo '=== Q03. Які зареєстровані пасажири не мають жодної картки? (LEFT JOIN … IS NULL)'
SELECT p.id, p.last_name, p.first_name, p.registered_at::date AS registered
FROM passengers AS p
LEFT JOIN cards AS c ON c.passenger_id = p.id
WHERE c.id IS NULL;

\echo '=== Q04. Які пасажири мають більше однієї картки? (GROUP BY, HAVING, string_agg)'
SELECT p.last_name, p.first_name, count(*) AS cards,
       string_agg(c.card_number, ', ' ORDER BY c.card_number) AS card_numbers
FROM passengers AS p
JOIN cards AS c ON c.passenger_id = p.id
GROUP BY p.id, p.last_name, p.first_name
HAVING count(*) > 1;

\echo '=== Q05. Які активні картки мають баланс нижчий за середній серед активних? (підзапит з агрегатом)'
SELECT c.card_number, c.balance,
       round((SELECT avg(balance) FROM cards WHERE status = 'active'), 2) AS avg_active_balance
FROM cards AS c
WHERE c.status = 'active'
  AND c.balance < (SELECT avg(balance) FROM cards WHERE status = 'active')
ORDER BY c.balance, c.card_number;

\echo '=== Q06. Де й коли кожна картка використовувалась востаннє? (DISTINCT ON)'
SELECT DISTINCT ON (c.card_number)
       c.card_number, t.validated_at, v.code AS validator, v.location, t.payment_method
FROM cards AS c
JOIN trips AS t ON t.card_id = c.id
JOIN validators AS v ON v.id = t.validator_id
ORDER BY c.card_number, t.validated_at DESC, t.id DESC;

\echo '=== Q07. Який середній і мінімальний інтервал між послідовними поїздками кожної картки? (LAG)'
WITH gaps AS (
    SELECT t.card_id,
           t.validated_at - lag(t.validated_at) OVER (PARTITION BY t.card_id
                                                      ORDER BY t.validated_at, t.id) AS gap
    FROM trips AS t
)
SELECT c.card_number,
       count(g.gap)                              AS intervals,
       date_trunc('minute', avg(g.gap))          AS avg_gap,
       date_trunc('minute', min(g.gap))          AS min_gap
FROM gaps AS g
JOIN cards AS c ON c.id = g.card_id
WHERE g.gap IS NOT NULL
GROUP BY c.card_number
ORDER BY avg_gap
LIMIT 10;

\echo '=== Q08. Як зростала сума поповнень картки 4000000000000010? (накопичувальна SUM OVER)'
SELECT t.created_at, t.channel, t.amount,
       sum(t.amount) OVER (ORDER BY t.created_at, t.id
                           ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS running_total,
       t.balance_after
FROM top_ups AS t
JOIN cards AS c ON c.id = t.card_id
WHERE c.card_number = '4000000000000010'
ORDER BY t.created_at, t.id;

\echo '=== Q09. Які валідатори найбільш завантажені в межах свого виду транспорту? (RANK, PARTITION BY)'
SELECT tt.name AS transport, v.code, v.location,
       count(t.id) AS trips,
       rank() OVER (PARTITION BY tt.id ORDER BY count(t.id) DESC) AS rank_in_type
FROM validators AS v
JOIN transport_types AS tt ON tt.id = v.transport_type_id
LEFT JOIN trips AS t ON t.validator_id = v.id
GROUP BY tt.id, tt.name, v.id, v.code, v.location
ORDER BY tt.name, rank_in_type, v.code;

\echo '=== Q10. Які години доби є піковими (понад 100 валідацій за весь період)? (date_trunc/extract, HAVING)'
SELECT extract(hour FROM t.validated_at AT TIME ZONE 'Europe/Kyiv')::int AS hour,
       count(*)                                                           AS trips,
       round(100.0 * count(*) / sum(count(*)) OVER (), 1)                 AS pct_of_all
FROM trips AS t
GROUP BY 1
HAVING count(*) > 100
ORDER BY trips DESC;

\echo '=== Q11. Яка частка пільгових поїздок (знижка або безкоштовно) на кожному виді транспорту? (представлення v_trip_facts)'
SELECT transport_type,
       count(*)                                             AS trips,
       count(*) FILTER (WHERE is_privileged)                AS privileged,
       round(100.0 * count(*) FILTER (WHERE is_privileged) / count(*), 1) AS privileged_pct
FROM v_trip_facts
GROUP BY transport_type
ORDER BY privileged_pct DESC;

\echo '=== Q12. Чиї проїзні закінчуються протягом 3 днів від моменту звіту? (інтервали дат)'
SELECT c.card_number, tp.code AS plan, s.valid_to, s.trips_left,
       date_trunc('hour', s.valid_to - :report_at::timestamptz) AS remaining
FROM card_subscriptions AS s
JOIN cards AS c ON c.id = s.card_id
JOIN tariff_plans AS tp ON tp.id = s.tariff_plan_id
WHERE :report_at::timestamptz >= s.valid_from
  AND s.valid_to > :report_at::timestamptz
  AND s.valid_to <= :report_at::timestamptz + interval '3 days'
ORDER BY s.valid_to;

\echo '=== Q13. Чи збігається кешований баланс кожної картки з журналами операцій? (CTE, FR05; очікується 0 розбіжностей)'
WITH topped AS (
    SELECT card_id, sum(amount) AS total FROM top_ups GROUP BY card_id
),
charged AS (
    SELECT card_id, sum(amount) AS total FROM charges GROUP BY card_id
),
reconciliation AS (
    SELECT c.card_number, c.balance,
           COALESCE(tp.total, 0) - COALESCE(ch.total, 0) AS journal_balance
    FROM cards AS c
    LEFT JOIN topped  AS tp ON tp.card_id = c.id
    LEFT JOIN charged AS ch ON ch.card_id = c.id
)
SELECT count(*)                                              AS cards_checked,
       count(*) FILTER (WHERE balance <> journal_balance)    AS mismatches,
       sum(balance)                                          AS total_cached,
       sum(journal_balance)                                  AS total_journal
FROM reconciliation;

\echo '=== Q14. Скільки поїздок у середньому припадає на один проїзний кожного плану? (агрегат над підзапитом)'
SELECT tp.code, tp.plan_type, count(*) AS sold,
       round(avg(x.trips), 1) AS avg_trips_per_plan,
       round(avg(ch.amount / NULLIF(x.trips, 0)), 2) AS avg_cost_per_trip
FROM card_subscriptions AS s
JOIN tariff_plans AS tp ON tp.id = s.tariff_plan_id
JOIN charges AS ch ON ch.subscription_id = s.id
CROSS JOIN LATERAL (SELECT count(*) AS trips FROM trips AS t WHERE t.subscription_id = s.id) AS x
GROUP BY tp.code, tp.plan_type
ORDER BY tp.code;

\echo '=== Q15. Як змінилась середня фактична оплата поїздки після підвищення тарифів 1 листопада? (CASE-групування)'
SELECT tt.name AS transport,
       CASE WHEN ch.charged_at < '2026-11-01' THEN 'до 01.11' ELSE 'з 01.11' END AS period,
       count(*)                     AS paid_trips,
       round(avg(ch.full_price), 2) AS avg_full_price,
       round(avg(ch.amount), 2)     AS avg_paid
FROM charges AS ch
JOIN trips AS t ON t.id = ch.trip_id
JOIN validators AS v ON v.id = t.validator_id
JOIN transport_types AS tt ON tt.id = v.transport_type_id
GROUP BY tt.name, 2
ORDER BY tt.name, 2;

\echo '=== Q16. Скільки коштів пасажири зекономили завдяки безкоштовним пересадкам (BR10)? (функція fn_current_fare у запиті)'
SELECT tt.name AS transport,
       count(*) AS transfers,
       sum(fn_current_fare(v.transport_type_id, t.validated_at)) AS saved_full_price
FROM trips AS t
JOIN validators AS v ON v.id = t.validator_id
JOIN transport_types AS tt ON tt.id = v.transport_type_id
WHERE t.payment_method = 'transfer'
GROUP BY tt.name
ORDER BY saved_full_price DESC;

\echo '=== Q17. Яка виписка картки 4000000000000009 за 10–20 жовтня (студентська пільга закінчилась 15.10)? (fn_card_statement)'
SELECT * FROM fn_card_statement('4000000000000009', '2026-10-10', '2026-10-21');

\echo '=== Q18. Яка історія змін статусів карток, крім видачі? (журнал аудиту)'
SELECT c.card_number, h.old_status, h.new_status, h.changed_by, h.reason
FROM card_status_history AS h
JOIN cards AS c ON c.id = h.card_id
WHERE h.old_status IS NOT NULL
ORDER BY h.id;

\echo '=== Добова виручка за останній тиждень (представлення v_daily_revenue)'
SELECT * FROM v_daily_revenue
WHERE day BETWEEN DATE '2026-11-15' AND DATE '2026-11-21'
ORDER BY day;

-- ===========================================================================
-- Операції зміни даних
-- ===========================================================================

\echo '=== M1. INSERT … RETURNING: реєстрація нового пасажира'
INSERT INTO passengers (last_name, first_name, birth_date, phone, email)
VALUES ('Вовк', 'Марта', '2001-02-03', '+380501110015', 'm.vovk@example.com')
RETURNING id, last_name, first_name, registered_at;

\echo '=== M2. UPDATE … RETURNING: пасажир змінив номер телефону'
UPDATE passengers
SET phone = '+380671110004'
WHERE phone = '+380501110004'
RETURNING id, last_name, phone;

\echo '=== M3. UPSERT (ON CONFLICT DO UPDATE): довідник пільг оновлюється за кодом'
-- Нова знижка діє на майбутні поїздки; історичні списання зберігають discount_pct на момент операції.
-- (xmax = 0) показує, рядок вставлено (true) чи оновлено (false).
INSERT INTO privilege_categories (code, name, discount_pct)
VALUES ('large_family', 'Член багатодітної родини', 30),
       ('veteran',      'Ветеран війни',            100)
ON CONFLICT (code) DO UPDATE
    SET discount_pct = EXCLUDED.discount_pct,
        name         = EXCLUDED.name
RETURNING code, discount_pct, (xmax = 0) AS inserted;

\echo '=== M4. DELETE … RETURNING: видалення архівного плану, який ніколи не продавався'
-- План із проданими проїзними видалити не можна (BR08, ON DELETE RESTRICT) —
-- тому умова явно перевіряє відсутність продажів, а такі плани лише деактивуються.
DELETE FROM tariff_plans AS tp
WHERE tp.code = 'M30_OLD'
  AND NOT EXISTS (SELECT 1 FROM card_subscriptions AS s WHERE s.tariff_plan_id = tp.id)
RETURNING tp.code, tp.name;
