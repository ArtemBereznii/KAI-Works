-- restore_check.sql — контрольні вибірки для порівняння вихідної та відновленої бази.
-- Запускається на обох базах (restore_test.sh), результати порівнюються diff.
-- Вивід не залежить від OID і часу запуску, лише від вмісту бази.

SET search_path = transit, public;
SET timezone = 'Europe/Kyiv';
\pset footer off

\echo '--- Об''єкти схеми transit'
SELECT 'tables' AS kind, count(*) FROM pg_tables WHERE schemaname = 'transit'
UNION ALL SELECT 'views', count(*) FROM pg_views WHERE schemaname = 'transit'
UNION ALL SELECT 'indexes', count(*) FROM pg_indexes WHERE schemaname = 'transit'
UNION ALL SELECT 'constraints', count(*) FROM pg_constraint c JOIN pg_namespace n ON n.oid = c.connamespace WHERE n.nspname = 'transit'
UNION ALL SELECT 'functions', count(*) FROM pg_proc p JOIN pg_namespace n ON n.oid = p.pronamespace WHERE n.nspname = 'transit' AND p.prokind = 'f'
UNION ALL SELECT 'procedures', count(*) FROM pg_proc p JOIN pg_namespace n ON n.oid = p.pronamespace WHERE n.nspname = 'transit' AND p.prokind = 'p'
UNION ALL SELECT 'triggers', count(*) FROM pg_trigger t JOIN pg_class c ON c.oid = t.tgrelid JOIN pg_namespace n ON n.oid = c.relnamespace WHERE n.nspname = 'transit' AND NOT t.tgisinternal;

\echo '--- Індекси'
SELECT tablename, indexname FROM pg_indexes WHERE schemaname = 'transit' ORDER BY 1, 2;

\echo '--- Кількість рядків'
SELECT 'privilege_categories' AS table_name, count(*) FROM privilege_categories
UNION ALL SELECT 'passengers', count(*) FROM passengers
UNION ALL SELECT 'cards', count(*) FROM cards
UNION ALL SELECT 'card_status_history', count(*) FROM card_status_history
UNION ALL SELECT 'transport_types', count(*) FROM transport_types
UNION ALL SELECT 'fares', count(*) FROM fares
UNION ALL SELECT 'tariff_plans', count(*) FROM tariff_plans
UNION ALL SELECT 'card_subscriptions', count(*) FROM card_subscriptions
UNION ALL SELECT 'validators', count(*) FROM validators
UNION ALL SELECT 'top_ups', count(*) FROM top_ups
UNION ALL SELECT 'trips', count(*) FROM trips
UNION ALL SELECT 'charges', count(*) FROM charges;

\echo '--- Контрольні суми'
SELECT (SELECT sum(balance) FROM cards)   AS sum_balance,
       (SELECT sum(amount) FROM top_ups)  AS sum_top_ups,
       (SELECT sum(amount) FROM charges)  AS sum_charges,
       (SELECT md5(string_agg(concat_ws('|', id, card_id, validator_id, validated_at, payment_method,
                                        subscription_id, transfer_from_trip_id), ',' ORDER BY id))
        FROM trips) AS trips_md5;

\echo '--- Звірка балансу з журналами (Q13)'
SELECT count(*) FILTER (WHERE c.balance <> COALESCE(tu.s, 0) - COALESCE(ch.s, 0)) AS mismatches
FROM cards AS c
LEFT JOIN (SELECT card_id, sum(amount) s FROM top_ups GROUP BY 1) AS tu ON tu.card_id = c.id
LEFT JOIN (SELECT card_id, sum(amount) s FROM charges GROUP BY 1) AS ch ON ch.card_id = c.id;

\echo '--- Основні запити: виручка за видами транспорту в листопаді (Q01) і представлення'
SELECT tt.name, count(t.id) AS trips, COALESCE(sum(ch.amount), 0) AS revenue
FROM transport_types AS tt
JOIN validators AS v ON v.transport_type_id = tt.id
LEFT JOIN trips AS t ON t.validator_id = v.id AND t.validated_at >= '2026-11-01' AND t.validated_at < '2026-12-01'
LEFT JOIN charges AS ch ON ch.trip_id = t.id
GROUP BY tt.name ORDER BY tt.name;
SELECT count(*) AS overview_rows, sum(balance) AS overview_balance FROM v_card_overview;
SELECT * FROM fn_card_statement('4000000000000009', '2026-10-15', '2026-10-17');

\echo '--- Обмеження й тригери працюють'
-- Результати повертаються рядками (stdout), а не NOTICE (stderr): restore_test.sh
-- зливає обидва потоки в один файл, і порядок NOTICE відносно рядків не гарантований.
BEGIN;
CREATE TEMP TABLE restore_probe (check_name text, result text) ON COMMIT DROP;
DO $$
DECLARE v_state text;
BEGIN
    BEGIN
        INSERT INTO cards (card_number, valid_until) VALUES ('4000000000000001', DATE '2030-01-01');
        v_state := 'вставлено';
    EXCEPTION WHEN OTHERS THEN GET STACKED DIAGNOSTICS v_state = RETURNED_SQLSTATE;
    END;
    INSERT INTO restore_probe VALUES ('дублікат номера картки', v_state);
    BEGIN
        DELETE FROM charges WHERE id = (SELECT min(id) FROM charges);
        v_state := 'видалено';
    EXCEPTION WHEN OTHERS THEN GET STACKED DIAGNOSTICS v_state = MESSAGE_TEXT;
    END;
    INSERT INTO restore_probe VALUES ('видалення з журналу', left(v_state, 4));
END $$;
SELECT check_name, result FROM restore_probe ORDER BY check_name;
ROLLBACK;

\echo '--- Права ролей'
SELECT grantee, table_name, string_agg(privilege_type, ',' ORDER BY privilege_type) AS privileges
FROM information_schema.role_table_grants
WHERE table_schema = 'transit' AND grantee LIKE 'transit\_%'
GROUP BY 1, 2 ORDER BY 1, 2;
SELECT r.routine_name, string_agg(g.grantee, ',' ORDER BY g.grantee) AS grantees
FROM information_schema.routines AS r
LEFT JOIN information_schema.routine_privileges AS g
       ON g.specific_name = r.specific_name AND g.grantee LIKE 'transit\_%'
WHERE r.routine_schema = 'transit'
GROUP BY 1 ORDER BY 1;
BEGIN;
SET LOCAL ROLE transit_analyst;
SELECT count(*) AS analyst_sees_trip_facts FROM v_trip_facts;
ROLLBACK;
