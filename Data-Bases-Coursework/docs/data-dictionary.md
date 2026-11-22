# К2. Словник даних схеми `transit`

Відповідає `sql/01_schema.sql` і `docs/er.mmd` (одна версія структури). Усі `id` — `GENERATED ALWAYS AS IDENTITY PRIMARY KEY`; усі FK — `ON DELETE RESTRICT`, `ON UPDATE NO ACTION` (сурогатні ключі не змінюються, тому каскад оновлення не потрібен).

## Нормалізація та свідомі відступи

- **1НФ:** кожне поле атомарне; телефон і e-mail — по одному на пасажира (інші контакти поза межами).
- **2НФ:** усі таблиці мають простий сурогатний ключ, часткових залежностей немає.
- **3НФ:** ціна поїздки винесена у `fares` (а не повторюється в кожній поїздці); характеристики плану — у `tariff_plans`; вид транспорту поїздки визначається через `validators`, а не дублюється в `trips`; прострочення картки виводиться з `valid_until`, а не зберігається окремим статусом.
- **Свідома денормалізація `cards.balance`:** баланс потрібен на кожній валідації, а сума журналів зростає з кожною операцією. Значення змінюється лише процедурами в тій самій транзакції, що й запис у `top_ups`/`charges`, під блокуванням `FOR UPDATE`; узгодженість перевіряє контрольний запит звірки (FR05).
- **Історичні значення, а не дублі:** `charges.full_price`, `discount_pct`, `balance_after` і `top_ups.balance_after` фіксують факт на момент операції — пізніша зміна тарифу чи пільги не повинна змінювати історію.

## privilege_categories — пільгові категорії

| Стовпець | Тип | Призначення й обмеження |
|---|---|---|
| id | smallint | Ідентифікатор |
| code | text | Код (`student`, `pensioner`…); NOT NULL, UNIQUE, `^[a-z_]{2,30}$` |
| name | text | Назва; NOT NULL, UNIQUE, не порожня |
| discount_pct | numeric(5,2) | Знижка, %; NOT NULL, `0 < x ≤ 100` |
| is_active | boolean | Чи надається пільга зараз; NOT NULL, DEFAULT true (замість видалення) |

## passengers — пасажири

| Стовпець | Тип | Призначення й обмеження |
|---|---|---|
| id | integer | Ідентифікатор |
| last_name, first_name | text | Прізвище, ім'я; NOT NULL, не порожні |
| middle_name | text | По батькові; NULL допустимий |
| birth_date | date | Дата народження; NOT NULL, ≥ 1900-01-01 |
| phone | text | Телефон; UNIQUE, NULL допустимий, `^\+380[0-9]{9}$` |
| email | text | E-mail; UNIQUE, NULL допустимий, базова перевірка формату |
| privilege_category_id | smallint | FK → privilege_categories; NULL — без пільги |
| privilege_valid_until | date | Кінець дії пільги (включно); NULL — безстроково; лише разом із пільгою |
| registered_at | timestamptz | Момент реєстрації; DEFAULT now() |

## cards — транспортні картки

| Стовпець | Тип | Призначення й обмеження |
|---|---|---|
| id | integer | Ідентифікатор |
| card_number | text | Номер на картці; NOT NULL, UNIQUE, рівно 16 цифр (BR01) |
| passenger_id | integer | FK → passengers; NULL — анонімна картка |
| status | text | `active` / `blocked` / `lost`; DEFAULT `active`; переходи — тригер (BR05) |
| balance | numeric(10,2) | Кешований баланс, грн; NOT NULL, DEFAULT 0, ≥ 0 (BR02) |
| issued_on | date | Дата видачі; DEFAULT current_date |
| valid_until | date | Останній день дії; > issued_on |
| status_changed_at | timestamptz | Момент останньої зміни статусу |

## card_status_history — історія статусів

| Стовпець | Тип | Призначення й обмеження |
|---|---|---|
| id | bigint | Ідентифікатор |
| card_id | integer | FK → cards; NOT NULL |
| old_status | text | Попередній статус; NULL для запису про видачу |
| new_status | text | Новий статус; NOT NULL; відрізняється від old_status |
| changed_at | timestamptz | DEFAULT clock_timestamp() — реальний момент, а не початок транзакції |
| changed_by | text | Роль, що виконала зміну; DEFAULT current_user |
| reason | text | Причина; NULL допустимий |

## transport_types — види транспорту

| Стовпець | Тип | Призначення й обмеження |
|---|---|---|
| id | smallint | Ідентифікатор |
| code | text | `bus`, `trolleybus`, `tram`, `metro`; UNIQUE |
| name | text | Назва українською; UNIQUE |

## fares — вартість разової поїздки

| Стовпець | Тип | Призначення й обмеження |
|---|---|---|
| id | integer | Ідентифікатор |
| transport_type_id | smallint | FK → transport_types; NOT NULL |
| price | numeric(8,2) | Повна ціна, грн; > 0 |
| valid_from | date | Перший день дії; NOT NULL |
| valid_to | date | День, з якого тариф уже не діє; NULL — безстроково; > valid_from |
| — | EXCLUDE | `(transport_type_id =, daterange(valid_from, valid_to, '[)') &&)` (BR06) |

## tariff_plans — тарифні плани

| Стовпець | Тип | Призначення й обмеження |
|---|---|---|
| id | smallint | Ідентифікатор |
| code | text | Код плану (`M30_ALL`); UNIQUE, `^[A-Z0-9_]{2,30}$` |
| name | text | Назва; не порожня |
| plan_type | text | `unlimited` — безлімітний на період; `trip_pack` — пакет поїздок |
| transport_type_id | smallint | FK → transport_types; NULL — діє на всіх видах |
| price | numeric(8,2) | Ціна плану без пільги; > 0 |
| duration_days | smallint | Строк дії від моменту купівлі, днів; 1–366 |
| trips_count | smallint | Кількість поїздок; лише для `trip_pack`, > 0 (BR14) |
| is_active | boolean | Чи продається план |

## card_subscriptions — проїзні на картці

| Стовпець | Тип | Призначення й обмеження |
|---|---|---|
| id | integer | Ідентифікатор |
| card_id | integer | FK → cards; NOT NULL |
| tariff_plan_id | smallint | FK → tariff_plans; NOT NULL |
| valid_from | timestamptz | Початок дії (включно) |
| valid_to | timestamptz | Кінець дії (не включно); > valid_from; вичерпаний пакет закривається моментом останньої поїздки |
| trips_left | smallint | Залишок поїздок пакета; ≥ 0; NULL для безлімітного |
| purchased_at | timestamptz | Момент купівлі |
| — | EXCLUDE | `(card_id =, tstzrange(valid_from, valid_to, '[)') &&)` (BR07) |

## validators — валідатори

| Стовпець | Тип | Призначення й обмеження |
|---|---|---|
| id | integer | Ідентифікатор |
| code | text | Інвентарний код `V-NNNNN`; UNIQUE |
| transport_type_id | smallint | FK → transport_types; NOT NULL |
| location | text | Маршрут / борт / станція; не порожнє |
| is_active | boolean | Чи приймає валідації |
| installed_on | date | Дата встановлення |

## top_ups — поповнення

| Стовпець | Тип | Призначення й обмеження |
|---|---|---|
| id | bigint | Ідентифікатор |
| card_id | integer | FK → cards; NOT NULL |
| amount | numeric(10,2) | Сума, грн; 1–5000 (BR03) |
| channel | text | `cash_desk` / `terminal` / `online` / `mobile_app` |
| payment_ref | text | Ідентифікатор платежу від каналу; NOT NULL, UNIQUE (BR09) |
| balance_after | numeric(10,2) | Баланс після поповнення; ≥ 0 |
| created_at | timestamptz | Момент поповнення |

## trips — поїздки

| Стовпець | Тип | Призначення й обмеження |
|---|---|---|
| id | bigint | Ідентифікатор |
| card_id | integer | FK → cards; NOT NULL |
| validator_id | integer | FK → validators; NOT NULL |
| validated_at | timestamptz | Момент валідації; UNIQUE разом із card_id (BR15) |
| payment_method | text | `balance` / `subscription` / `transfer` / `free` |
| subscription_id | integer | FK → card_subscriptions; заповнений тоді й лише тоді, коли метод `subscription` (BR13) |

Примітка: `UNIQUE (card_id, validated_at)` створює B-tree-індекс, який уже обслуговує запит «останні поїздки картки». У дослідженні продуктивності (К4) це треба врахувати: порівняння «до/після» доцільно будувати на запитах за інтервалом часу та за валідатором.

## charges — списання

| Стовпець | Тип | Призначення й обмеження |
|---|---|---|
| id | bigint | Ідентифікатор |
| card_id | integer | FK → cards; NOT NULL |
| charge_type | text | `trip` / `plan_purchase` |
| trip_id | bigint | FK → trips, UNIQUE; лише для `trip` (BR12) |
| subscription_id | integer | FK → card_subscriptions, UNIQUE; лише для `plan_purchase` (BR12) |
| full_price | numeric(10,2) | Ціна без знижки на момент операції; > 0 |
| discount_pct | numeric(5,2) | Застосована знижка; 0 ≤ x < 100 (100 % — поїздка `free` без списання) |
| amount | numeric(10,2) | Фактично списано; > 0, ≤ full_price |
| balance_after | numeric(10,2) | Баланс після списання; ≥ 0 |
| charged_at | timestamptz | Момент списання |
