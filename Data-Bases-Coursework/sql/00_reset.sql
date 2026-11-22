-- 00_reset.sql — повернення бази до чистого стану перед повним прогоном.
-- Видаляє схему transit з усіма об'єктами та створює її заново.
-- Ролі є об'єктами кластера, тому їх перестворює 02_roles.sql.

\set QUIET on
SET client_min_messages = warning;

DROP SCHEMA IF EXISTS transit CASCADE;

-- btree_gist потрібне для EXCLUDE-обмежень, що поєднують
-- рівність (=) за ключем і перетин (&&) діапазонів дат/часу.
CREATE EXTENSION IF NOT EXISTS btree_gist WITH SCHEMA public;

CREATE SCHEMA transit;
COMMENT ON SCHEMA transit IS
    'ІС електронних проїзних документів (курсова «Бази даних», варіант 67)';

\set QUIET off
\echo 'Схему transit скинуто та створено заново'
