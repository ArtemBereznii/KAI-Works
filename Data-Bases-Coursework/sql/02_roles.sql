-- 02_roles.sql — ролі користувачів і права на таблиці.
-- Ролі — групові (NOLOGIN): облікові записи конкретних працівників отримують
-- членство в них. Для перевірки прав використовується SET ROLE.
-- Права на функції/процедури видаються в 03_functions.sql, на представлення — у 05_views.sql,
-- бо на момент виконання цього скрипта цих об'єктів ще немає.

SET search_path = transit, public;

-- Ролі є об'єктами кластера: створюємо лише якщо їх ще немає (ідемпотентність).
DO $$
DECLARE r text;
BEGIN
    FOREACH r IN ARRAY ARRAY['transit_cashier', 'transit_validator', 'transit_analyst'] LOOP
        IF NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = r) THEN
            EXECUTE format('CREATE ROLE %I NOLOGIN', r);
        END IF;
    END LOOP;
END
$$;

COMMENT ON ROLE transit_cashier   IS 'Касир сервісного центру: пасажири, картки, поповнення, продаж проїзних';
COMMENT ON ROLE transit_validator IS 'ПЗ валідатора: лише реєстрація поїздки';
COMMENT ON ROLE transit_analyst   IS 'Аналітик: лише читання звітних представлень';

-- За замовчуванням PUBLIC може виконувати будь-яку нову функцію. ALTER DEFAULT PRIVILEGES
-- IN SCHEMA це право не забирає (на рівні схеми можна лише додавати права), тому
-- після створення функцій 03_functions.sql і 04_triggers.sql явно виконують
-- REVOKE EXECUTE ON ALL ROUTINES IN SCHEMA transit FROM PUBLIC.
REVOKE ALL ON SCHEMA transit FROM PUBLIC;

GRANT USAGE ON SCHEMA transit TO transit_cashier, transit_validator, transit_analyst;

-- Касир: читає довідники й стан карток; пасажирів реєструє та редагує напряму
-- (грошей ця операція не стосується). Картки, баланс, проїзні та журнали
-- змінюються лише процедурами (SECURITY DEFINER), прямих прав на них немає.
GRANT SELECT ON privilege_categories, transport_types, fares, tariff_plans, validators,
                passengers, cards, card_subscriptions, card_status_history
    TO transit_cashier;
GRANT INSERT ON passengers TO transit_cashier;
GRANT UPDATE (last_name, first_name, middle_name, phone, email,
              privilege_category_id, privilege_valid_until)
    ON passengers TO transit_cashier;

-- Валідатор і аналітик прямих прав на таблиці не мають.
