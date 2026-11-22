# Курсова «Бази даних» — ІС електронних проїзних документів (варіант 67)

База даних обліку транспортних карток, поповнень, проїзних, поїздок і списань коштів у PostgreSQL.

## Середовище

| Компонент | Версія |
|---|---|
| PostgreSQL (сервер) | 18.7 (Docker-образ `postgres:18`, Debian) |
| Клієнт | psql 18.7 з того самого контейнера |
| Розширення | `btree_gist` (EXCLUDE-обмеження на неперетин періодів) |
| Схема | `transit`, база `transit_db`, часовий пояс `Europe/Kyiv` |

Паролі в репозиторії не зберігаються: пароль суперкористувача задається у локальному файлі `.env` (шаблон — `.env.example`), який виключено з git.

## Запуск

```sh
cp .env.example .env          # і вписати власний POSTGRES_PASSWORD
docker compose up -d --wait   # PostgreSQL 18 на 127.0.0.1:5432
./run_all.sh                  # повний прогін sql/00_*.sql … sql/NN_*.sql
```

- Окремі скрипти: `./run_all.sh 00_reset.sql 01_schema.sql`.
- Власний клієнт psql замість контейнерного: `PSQL_MODE=local ./run_all.sh` (підключення — через `PGHOST`, `PGUSER`, `~/.pgpass`).
- Інтерактивний сеанс: `docker compose exec db psql -U postgres -d transit_db`.

Кожен скрипт виконується з `-v ON_ERROR_STOP=1`; повний прогін ідемпотентний, бо `00_reset.sql` перестворює схему.

## Очікуваний результат

| Скрипт | Результат |
|---|---|
| `00_reset.sql` | Схема `transit` порожня, `btree_gist` встановлене, часовий пояс БД `Europe/Kyiv` |
| `01_schema.sql` | 12 таблиць з ключами, обмеженнями та 2 додатковими індексами |
| `02_roles.sql` | Ролі `transit_cashier`, `transit_validator`, `transit_analyst` (NOLOGIN) і права на таблиці |
| `03_functions.sql` | 4 функції (`fn_actor`, `fn_current_fare`, `fn_card_discount`, `fn_card_statement`) і 5 процедур (`proc_issue_card`, `proc_set_card_status`, `proc_top_up`, `proc_buy_plan`, `proc_register_trip`); права EXECUTE за ролями |
| `04_triggers.sql` | 6 тригерів: перевірка переходу статусу, аудит статусу (INSERT/UPDATE), незмінність трьох журналів |
| `05_views.sql` | `v_card_overview` (каса), `v_trip_facts`, `v_daily_revenue` (аналітик) |
| `06_seed.sql` | 14 пасажирів, 18 карток, 58 поповнень, 28 проїзних, 1323 поїздки, 577 списань; повідомлення `Згенеровано поїздок: 1323, відхилено: 0` |
| `07_queries.sql` | 18 запитів Q01–Q18 + добова виручка; операції зміни M1–M4; у Q13 — `mismatches = 0` |

Скрипт завершується рядком `=== Готово: N скрипт(ів) виконано без помилок ===`. Наповнення відтворюване (фіксований `setseed`), тому числа збігаються при кожному повному прогоні.

Окремо `06_seed.sql` чи `07_queries.sql` повторно не запускаються (унікальні ключі) — лише в повному прогоні, що починається з `00_reset.sql`.

## Модель доступу

| Роль | Може | Не може |
|---|---|---|
| `transit_cashier` | читати довідники, картки, проїзні; реєструвати й редагувати пасажирів; викликати `proc_issue_card`, `proc_set_card_status`, `proc_top_up`, `proc_buy_plan`, `fn_card_statement`, `fn_current_fare`; читати `v_card_overview` | змінювати картки, баланс, проїзні та журнали напряму; реєструвати поїздки |
| `transit_validator` | `CALL proc_register_trip` | читати будь-які таблиці й представлення; поповнювати картки |
| `transit_analyst` | читати `v_trip_facts`, `v_daily_revenue` | читати базові таблиці та персональні дані; виконувати функції; змінювати дані |

Процедури оголошено `SECURITY DEFINER`, тож роль працює з таблицями лише через них. Перевірка: `SET ROLE transit_cashier;` … `RESET ROLE;`.

## Документація

- [docs/task.md](docs/task.md) — чернетка завдання (форма Додатка Б)
- [docs/requirements.md](docs/requirements.md) — процеси, бізнес-правила, вимоги
- [docs/data-dictionary.md](docs/data-dictionary.md) — словник даних і нормалізація
- [docs/er.mmd](docs/er.mmd) — ER-діаграма (Mermaid)
