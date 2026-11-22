#!/usr/bin/env bash
# Послідовний прогін усіх SQL-скриптів проєкту (sql/NN_*.sql) з зупинкою на першій помилці.
#
# За замовчуванням psql запускається всередині контейнера docker compose (сервіс db),
# тож локально встановлений psql не потрібен.
# Щоб використати власний клієнт: PSQL_MODE=local ./run_all.sh
# (параметри підключення — через стандартні PGHOST/PGPORT/PGUSER/PGDATABASE і ~/.pgpass).
#
# Окремі скрипти можна передати аргументами: ./run_all.sh 00_reset.sql 01_schema.sql
set -euo pipefail
cd "$(dirname "$0")"

DB="${PGDATABASE:-transit_db}"
MODE="${PSQL_MODE:-docker}"

if [ "$#" -gt 0 ]; then
  files=("$@")
else
  files=()
  for f in sql/[0-9][0-9]_*.sql; do files+=("$(basename "$f")"); done
fi

for f in "${files[@]}"; do
  echo "=== ${f} ==="
  if [ "$MODE" = "local" ]; then
    psql -X -v ON_ERROR_STOP=1 -d "$DB" -f "sql/${f}"
  else
    # MSYS_NO_PATHCONV: щоб Git Bash на Windows не переписував шлях /work/sql
    MSYS_NO_PATHCONV=1 docker compose exec -T db \
      psql -X -v ON_ERROR_STOP=1 -U postgres -d "$DB" -f "/work/sql/${f}"
  fi
done

echo "=== Готово: ${#files[@]} скрипт(ів) виконано без помилок ==="
