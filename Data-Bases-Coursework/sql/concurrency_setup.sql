-- concurrency_setup.sql — підготовка до сценаріїв конкурентного доступу (BR11, protocols/concurrency.md).
-- Створює дві анонімні картки з балансом 30.00: вистачає рівно на одну поїздку
-- автобусом (20.00), але не на дві.
--   4777000000000001 — сценарій 1 (сеанс А виконує COMMIT)
--   4777000000000002 — сценарій 2 (сеанс А виконує ROLLBACK)
-- Запуск: ./run_all.sh concurrency_setup.sql
-- Повторний експеримент — після повного прогону ./run_all.sh (журнали незмінні, BR17).

SET search_path = transit, public;

DO $$
DECLARE v_num text; v_id integer; v_tid bigint; v_bal numeric;
BEGIN
    FOREACH v_num IN ARRAY ARRAY['4777000000000001', '4777000000000002'] LOOP
        IF EXISTS (SELECT 1 FROM cards WHERE card_number = v_num) THEN
            RAISE NOTICE 'Картка % вже існує, баланс %', v_num,
                (SELECT balance FROM cards WHERE card_number = v_num);
            CONTINUE;
        END IF;
        CALL proc_issue_card(v_num, NULL, v_id);
        CALL proc_top_up(v_num, 30, 'cash_desk', 'CONC-SETUP-' || right(v_num, 1), v_tid, v_bal);
        RAISE NOTICE 'Картку % створено, баланс %', v_num, v_bal;
    END LOOP;
END $$;

SELECT card_number, status, balance FROM cards WHERE card_number LIKE '4777%' ORDER BY 1;
