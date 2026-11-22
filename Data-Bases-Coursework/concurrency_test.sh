#!/usr/bin/env bash
# Автоматизований прогін сценаріїв конкурентного доступу (BR11) двома окремими
# підключеннями psql (два процеси, різні pg_backend_pid) + третій сеанс-спостерігач.
# Команди подаються в psql із паузами, тому сеанси справді перетинаються в часі.
# Перед запуском: ./run_all.sh && ./run_all.sh concurrency_setup.sql
# Ручний варіант того самого сценарію — protocols/concurrency.md.
#
# Використання: ./concurrency_test.sh 1   (сеанс А — COMMIT, картка 4777000000000001)
#               ./concurrency_test.sh 2   (сеанс А — ROLLBACK, картка 4777000000000002)
set -euo pipefail
cd "$(dirname "$0")"

SCENARIO="${1:-1}"
if [ "$SCENARIO" = "1" ]; then CARD="4777000000000001"; FINISH="COMMIT"; else CARD="4777000000000002"; FINISH="ROLLBACK"; fi
OUT="concurrency_out"
mkdir -p "$OUT"

psql_session() { MSYS_NO_PATHCONV=1 docker compose exec -T db psql -X -U postgres -d transit_db -e; }
ts() { echo "\\echo ### $1: $(date +%H:%M:%S.%3N)"; }

# Сеанс А: блокує картку, тримає транзакцію 8 с, потім COMMIT/ROLLBACK
{
  echo "\\timing on"
  echo "SELECT pg_backend_pid() AS session_a_pid;"
  echo "SET search_path = transit, public;"
  echo "SET ROLE transit_validator;"
  echo "BEGIN;"
  echo "SELECT clock_timestamp() AS a_call_start;"
  echo "CALL proc_register_trip('$CARD', 'V-10001', NULL, NULL, NULL, NULL);"
  echo "SELECT clock_timestamp() AS a_call_end;"
  sleep 8
  echo "SELECT clock_timestamp() AS a_finish_at;"
  echo "$FINISH;"
  sleep 2
  echo "RESET ROLE;"
  echo "SELECT c.card_number, c.balance, (SELECT count(*) FROM trips t WHERE t.card_id = c.id) AS trips,"
  echo "       (SELECT count(*) FROM charges h WHERE h.card_id = c.id) AS charges"
  echo "FROM cards c WHERE c.card_number = '$CARD';"
} | psql_session > "$OUT/s${SCENARIO}_session_a.txt" 2>&1 &
PID_A=$!

sleep 2

# Сеанс Б: те саме списання з тієї самої картки, поки А тримає блокування
{
  echo "\\timing on"
  echo "SELECT pg_backend_pid() AS session_b_pid;"
  echo "SET search_path = transit, public;"
  echo "SET ROLE transit_validator;"
  echo "BEGIN;"
  echo "SET LOCAL lock_timeout = '60s';"
  echo "SELECT clock_timestamp() AS b_call_start;"
  echo "CALL proc_register_trip('$CARD', 'V-10001', NULL, NULL, NULL, NULL);"
  echo "SELECT clock_timestamp() AS b_call_end;"
  echo "COMMIT;"
} | psql_session > "$OUT/s${SCENARIO}_session_b.txt" 2>&1 &
PID_B=$!

sleep 2

# Спостерігач: стан сеансів і блокування, поки Б чекає
{
  echo "SELECT clock_timestamp() AS observed_at;"
  echo "SELECT pid, usename, wait_event_type, wait_event, state, left(regexp_replace(query, '\\s+', ' ', 'g'), 70) AS query"
  echo "FROM pg_stat_activity WHERE datname = 'transit_db' AND backend_type = 'client backend' AND pid <> pg_backend_pid() ORDER BY pid;"
  echo "SELECT pid, pg_blocking_pids(pid) AS blocked_by FROM pg_stat_activity"
  echo "WHERE datname = 'transit_db' AND cardinality(pg_blocking_pids(pid)) > 0;"
} | psql_session > "$OUT/s${SCENARIO}_observer.txt" 2>&1

wait "$PID_A" || true
wait "$PID_B" || true

for f in session_a session_b observer; do
  echo "================ сценарій ${SCENARIO}: ${f} ================"
  cat "$OUT/s${SCENARIO}_${f}.txt"
done
