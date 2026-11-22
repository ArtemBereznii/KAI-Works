-- 09_perf.sql — дослідження продуктивності: EXPLAIN (ANALYZE, BUFFERS) до та після індексу.
-- Не входить у стандартний прогін; запуск після повного прогону: ./run_all.sh 09_perf.sql
--
-- Набір даних. 2000 анонімних карток 48…, кожна з 9 місячними проїзними M30_ALL
-- (2026-01-01 … 2026-09-28; для кожного: поповнення 1200 → купівля 1200), та :perf_trips
-- поїздок за проїзними у випадкові моменти цього періоду. Поїздки вставляються в
-- хронологічному порядку, як їх записують валідатори. Генерація виконується напряму
-- INSERT … SELECT (процедура на кожну поїздку була б надто повільною), але дані
-- узгоджені: звірка балансу з журналами дає 0 розбіжностей. Період не перетинається
-- з даними 06_seed.sql. Якщо набір уже є, генерація пропускається.
--
-- Експерименти (одна зміна за раз, кожен стан — 3 запуски поспіль):
--   E1. Усі поїздки за одну добу (вибірка за інтервалом часу) — BRIN (validated_at).
--   E2. Навантаження одного валідатора за місяць — B-tree (validator_id, validated_at).
--   E0. Останні 10 поїздок картки — вже обслуговується UNIQUE (card_id, validated_at), для порівняння.

\set perf_trips 500000
SET search_path = transit, public;
SET timezone = 'Europe/Kyiv';
\timing on

SELECT version();

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM cards WHERE card_number LIKE '48%') THEN
        RAISE NOTICE 'Набір для дослідження вже створено, генерацію пропущено';
    END IF;
END $$;

SELECT NOT EXISTS (SELECT 1 FROM cards WHERE card_number LIKE '48%') AS need_generation \gset

\if :need_generation
BEGIN;

INSERT INTO cards (card_number, issued_on, valid_until, status_changed_at)
SELECT '48' || lpad(n::text, 14, '0'), DATE '2025-12-01', DATE '2030-12-01', timestamptz '2025-12-01 09:00'
FROM generate_series(1, 2000) AS n;

CREATE TEMP TABLE perf_periods AS
SELECT c.id AS card_id, c.card_number, k,
       timestamptz '2026-01-01 00:00' + make_interval(days => 30 * k) AS period_start
FROM cards AS c
CROSS JOIN generate_series(0, 8) AS k
WHERE c.card_number LIKE '48%';

INSERT INTO top_ups (card_id, amount, channel, payment_ref, balance_after, created_at)
SELECT card_id, 1200, 'online', 'PERF-' || card_number || '-' || k, 1200, period_start - interval '1 hour'
FROM perf_periods;

INSERT INTO card_subscriptions (card_id, tariff_plan_id, valid_from, valid_to, trips_left, purchased_at)
SELECT pp.card_id, tp.id, pp.period_start, pp.period_start + interval '30 days', NULL, pp.period_start
FROM perf_periods AS pp
JOIN tariff_plans AS tp ON tp.code = 'M30_ALL';

INSERT INTO charges (card_id, charge_type, subscription_id, full_price, discount_pct, amount, balance_after, charged_at)
SELECT s.card_id, 'plan_purchase', s.id, 1200, 0, 1200, 0, s.valid_from
FROM card_subscriptions AS s
JOIN cards AS c ON c.id = s.card_id
WHERE c.card_number LIKE '48%';

SELECT setseed(0.67);

-- Поїздки: випадкова картка, валідатор і момент у межах 270 діб; у порядку часу.
INSERT INTO trips (card_id, validator_id, validated_at, payment_method, subscription_id)
SELECT g.card_id, g.validator_id, g.validated_at, 'subscription', s.id
FROM (
    SELECT (SELECT min(id) FROM cards WHERE card_number LIKE '48%') + floor(random() * 2000)::int AS card_id,
           (ARRAY(SELECT id FROM validators WHERE is_active ORDER BY code))[1 + floor(random() * 11)::int] AS validator_id,
           timestamptz '2026-01-01 00:00' + random() * interval '270 days' AS validated_at
    FROM generate_series(1, :perf_trips)
) AS g
JOIN card_subscriptions AS s
  ON s.card_id = g.card_id AND g.validated_at >= s.valid_from AND g.validated_at < s.valid_to
ORDER BY g.validated_at
ON CONFLICT DO NOTHING;

DROP TABLE perf_periods;
COMMIT;
\endif

ANALYZE;

\echo '=== Обсяг даних'
SELECT count(*) AS trips_total,
       pg_size_pretty(pg_relation_size('trips')) AS trips_heap,
       pg_size_pretty(pg_indexes_size('trips')) AS trips_indexes
FROM trips;

\echo '=== Звірка балансу після генерації (очікується 0)'
SELECT count(*) AS mismatches
FROM cards AS c
WHERE c.balance <> COALESCE((SELECT sum(amount) FROM top_ups WHERE card_id = c.id), 0)
                 - COALESCE((SELECT sum(amount) FROM charges WHERE card_id = c.id), 0);

-- Початковий стан: індексів досліду немає
DROP INDEX IF EXISTS trips_validated_at_brin;
DROP INDEX IF EXISTS trips_validator_time_idx;
ANALYZE trips;

\echo '=== Параметри: E1 — доба 2026-05-12; E2 — валідатор V-40003, травень 2026; E0 — картка 4800000000001000'

\echo '=== E1 ДО: поїздки за добу (запуски 1, 2, 3)'
EXPLAIN (ANALYZE, BUFFERS)
SELECT count(*), count(DISTINCT card_id) FROM trips
WHERE validated_at >= '2026-05-12' AND validated_at < '2026-05-13';
EXPLAIN (ANALYZE, BUFFERS)
SELECT count(*), count(DISTINCT card_id) FROM trips
WHERE validated_at >= '2026-05-12' AND validated_at < '2026-05-13';
EXPLAIN (ANALYZE, BUFFERS)
SELECT count(*), count(DISTINCT card_id) FROM trips
WHERE validated_at >= '2026-05-12' AND validated_at < '2026-05-13';

\echo '=== E2 ДО: навантаження валідатора за місяць (запуски 1, 2, 3)'
EXPLAIN (ANALYZE, BUFFERS)
SELECT date_trunc('day', t.validated_at) AS day, count(*)
FROM trips AS t
WHERE t.validator_id = (SELECT id FROM validators WHERE code = 'V-40003')
  AND t.validated_at >= '2026-05-01' AND t.validated_at < '2026-06-01'
GROUP BY 1 ORDER BY 1;
EXPLAIN (ANALYZE, BUFFERS)
SELECT date_trunc('day', t.validated_at) AS day, count(*)
FROM trips AS t
WHERE t.validator_id = (SELECT id FROM validators WHERE code = 'V-40003')
  AND t.validated_at >= '2026-05-01' AND t.validated_at < '2026-06-01'
GROUP BY 1 ORDER BY 1;
EXPLAIN (ANALYZE, BUFFERS)
SELECT date_trunc('day', t.validated_at) AS day, count(*)
FROM trips AS t
WHERE t.validator_id = (SELECT id FROM validators WHERE code = 'V-40003')
  AND t.validated_at >= '2026-05-01' AND t.validated_at < '2026-06-01'
GROUP BY 1 ORDER BY 1;

\echo '=== E0: останні 10 поїздок картки (наявний UNIQUE-індекс)'
EXPLAIN (ANALYZE, BUFFERS)
SELECT t.validated_at, t.validator_id, t.payment_method
FROM trips AS t
WHERE t.card_id = (SELECT id FROM cards WHERE card_number = '4800000000001000')
ORDER BY t.validated_at DESC
LIMIT 10;

\echo '=== Зміна 1: BRIN-індекс за часом валідації'
CREATE INDEX trips_validated_at_brin ON trips USING brin (validated_at);
ANALYZE trips;

\echo '=== E1 ПІСЛЯ (запуски 1, 2, 3)'
EXPLAIN (ANALYZE, BUFFERS)
SELECT count(*), count(DISTINCT card_id) FROM trips
WHERE validated_at >= '2026-05-12' AND validated_at < '2026-05-13';
EXPLAIN (ANALYZE, BUFFERS)
SELECT count(*), count(DISTINCT card_id) FROM trips
WHERE validated_at >= '2026-05-12' AND validated_at < '2026-05-13';
EXPLAIN (ANALYZE, BUFFERS)
SELECT count(*), count(DISTINCT card_id) FROM trips
WHERE validated_at >= '2026-05-12' AND validated_at < '2026-05-13';

\echo '=== Зміна 2: B-tree (validator_id, validated_at)'
CREATE INDEX trips_validator_time_idx ON trips (validator_id, validated_at);
ANALYZE trips;

\echo '=== E2 ПІСЛЯ (запуски 1, 2, 3)'
EXPLAIN (ANALYZE, BUFFERS)
SELECT date_trunc('day', t.validated_at) AS day, count(*)
FROM trips AS t
WHERE t.validator_id = (SELECT id FROM validators WHERE code = 'V-40003')
  AND t.validated_at >= '2026-05-01' AND t.validated_at < '2026-06-01'
GROUP BY 1 ORDER BY 1;
EXPLAIN (ANALYZE, BUFFERS)
SELECT date_trunc('day', t.validated_at) AS day, count(*)
FROM trips AS t
WHERE t.validator_id = (SELECT id FROM validators WHERE code = 'V-40003')
  AND t.validated_at >= '2026-05-01' AND t.validated_at < '2026-06-01'
GROUP BY 1 ORDER BY 1;
EXPLAIN (ANALYZE, BUFFERS)
SELECT date_trunc('day', t.validated_at) AS day, count(*)
FROM trips AS t
WHERE t.validator_id = (SELECT id FROM validators WHERE code = 'V-40003')
  AND t.validated_at >= '2026-05-01' AND t.validated_at < '2026-06-01'
GROUP BY 1 ORDER BY 1;

\echo '=== Вартість індексів'
SELECT indexrelname AS index_name, pg_size_pretty(pg_relation_size(indexrelid)) AS size
FROM pg_stat_user_indexes
WHERE relname = 'trips'
ORDER BY pg_relation_size(indexrelid) DESC;

\timing off
