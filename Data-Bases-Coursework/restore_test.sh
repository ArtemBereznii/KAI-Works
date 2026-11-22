#!/usr/bin/env bash
# Перевірка резервного копіювання: логічний дамп transit_db → відновлення в окрему
# порожню базу course_restore → однакові контрольні вибірки на обох базах → diff.
# Виконується в контейнері docker compose (сервіс db). Ролі transit_* існують у тому
# самому кластері, тому права відновлюються разом з об'єктами (--no-owner не потрібен).
set -euo pipefail
cd "$(dirname "$0")"

SRC="transit_db"
DST="course_restore"
DUMP="/tmp/transit_db.dump"
OUT="restore_out"
mkdir -p "$OUT"

dc() { MSYS_NO_PATHCONV=1 docker compose exec -T db "$@"; }

echo "=== Версії"
dc pg_dump --version
dc pg_restore --version

echo "=== 1. Дамп ${SRC} (custom format)"
time dc pg_dump -U postgres -d "$SRC" -Fc -f "$DUMP"
dc ls -l "$DUMP"
echo "--- Зміст дампу (кількість записів TOC за типом)"
dc pg_restore -l "$DUMP" | grep -v '^;' | awk '{print $4, $5}' | sort | uniq -c

echo "=== 2. Нова порожня база ${DST}"
dc dropdb -U postgres --if-exists "$DST"
dc createdb -U postgres "$DST"

echo "=== 3. Відновлення"
time dc pg_restore -U postgres --exit-on-error -d "$DST" "$DUMP"

echo "=== 4. Контрольні вибірки на обох базах"
dc psql -X -v ON_ERROR_STOP=1 -U postgres -d "$SRC" -f /work/sql/restore_check.sql > "$OUT/check_${SRC}.txt" 2>&1
dc psql -X -v ON_ERROR_STOP=1 -U postgres -d "$DST" -f /work/sql/restore_check.sql > "$OUT/check_${DST}.txt" 2>&1

echo "=== 5. Порівняння"
if diff -u "$OUT/check_${SRC}.txt" "$OUT/check_${DST}.txt"; then
  echo "Результати контрольних вибірок ідентичні ($(wc -l < "$OUT/check_${DST}.txt") рядків)"
else
  echo "Є розбіжності між вихідною та відновленою базою" >&2
  exit 1
fi
