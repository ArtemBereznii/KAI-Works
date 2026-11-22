-- concurrency_setup.sql — підготовка до сценарію конкурентного доступу (BR11, protocols/concurrency.md).
-- Створює анонімну картку 4777000000000001 з балансом 30.00: вистачає рівно на одну
-- поїздку автобусом (20.00), але не на дві. Запуск: ./run_all.sh concurrency_setup.sql
-- Повторний експеримент — після повного прогону ./run_all.sh (журнали незмінні, BR17).

SET search_path = transit, public;

DO $$
DECLARE v_id integer; v_tid bigint; v_bal numeric;
BEGIN
    IF EXISTS (SELECT 1 FROM cards WHERE card_number = '4777000000000001') THEN
        RAISE NOTICE 'Картка 4777000000000001 вже існує, баланс %',
            (SELECT balance FROM cards WHERE card_number = '4777000000000001');
        RETURN;
    END IF;
    CALL proc_issue_card('4777000000000001', NULL, v_id);
    CALL proc_top_up('4777000000000001', 30, 'cash_desk', 'CONC-SETUP-1', v_tid, v_bal);
    RAISE NOTICE 'Картку 4777000000000001 створено, баланс %', v_bal;
END $$;

SELECT card_number, status, balance FROM cards WHERE card_number = '4777000000000001';
