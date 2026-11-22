-- 04_triggers.sql — тригери.
--   BEFORE UPDATE cards.status  — перевірка дозволеного переходу статусу (BR05);
--   AFTER INSERT / UPDATE status — аудит у card_status_history;
--   BEFORE UPDATE/DELETE журналів — незмінність історії операцій (BR17).

SET search_path = transit, public;

-- ---------------------------------------------------------------------------
-- BR05: дозволені переходи статусу картки
--   active  → blocked | lost
--   blocked → active  | lost
--   lost    → (кінцевий стан)
-- ---------------------------------------------------------------------------
CREATE FUNCTION fn_trg_card_status_guard() RETURNS trigger
LANGUAGE plpgsql
AS $$
BEGIN
    IF NOT (   (OLD.status = 'active'  AND NEW.status IN ('blocked', 'lost'))
            OR (OLD.status = 'blocked' AND NEW.status IN ('active', 'lost'))) THEN
        RAISE EXCEPTION 'BR05: перехід статусу картки % → % заборонено', OLD.status, NEW.status
            USING HINT = 'Статус lost є кінцевим; для втраченої картки видається нова';
    END IF;
    NEW.status_changed_at := now();
    RETURN NEW;
END;
$$;

CREATE TRIGGER trg_cards_status_guard
    BEFORE UPDATE OF status ON cards
    FOR EACH ROW
    WHEN (OLD.status IS DISTINCT FROM NEW.status)
    EXECUTE FUNCTION fn_trg_card_status_guard();

-- ---------------------------------------------------------------------------
-- Аудит статусу: хто, коли, з якого на який статус і чому.
-- Причину процедури передають через параметр транзакції transit.status_reason.
-- ---------------------------------------------------------------------------
CREATE FUNCTION fn_trg_card_status_audit() RETURNS trigger
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO card_status_history (card_id, old_status, new_status, changed_by, reason)
    VALUES (NEW.id,
            CASE WHEN TG_OP = 'UPDATE' THEN OLD.status END,
            NEW.status,
            fn_actor(),
            NULLIF(current_setting('transit.status_reason', true), ''));
    RETURN NULL;   -- результат AFTER-тригера рядка ігнорується
END;
$$;

CREATE TRIGGER trg_cards_status_audit_ins
    AFTER INSERT ON cards
    FOR EACH ROW
    EXECUTE FUNCTION fn_trg_card_status_audit();

CREATE TRIGGER trg_cards_status_audit_upd
    AFTER UPDATE OF status ON cards
    FOR EACH ROW
    WHEN (OLD.status IS DISTINCT FROM NEW.status)
    EXECUTE FUNCTION fn_trg_card_status_audit();

-- ---------------------------------------------------------------------------
-- BR17: записи журналів операцій незмінні — помилкову операцію виправляють
-- новою (компенсуючою) операцією, а не редагуванням історії.
-- ---------------------------------------------------------------------------
CREATE FUNCTION fn_trg_journal_immutable() RETURNS trigger
LANGUAGE plpgsql
AS $$
BEGIN
    RAISE EXCEPTION 'BR17: записи журналу % не можна змінювати чи видаляти (%)', TG_TABLE_NAME, TG_OP;
END;
$$;

CREATE TRIGGER trg_top_ups_immutable
    BEFORE UPDATE OR DELETE ON top_ups
    FOR EACH ROW EXECUTE FUNCTION fn_trg_journal_immutable();

CREATE TRIGGER trg_charges_immutable
    BEFORE UPDATE OR DELETE ON charges
    FOR EACH ROW EXECUTE FUNCTION fn_trg_journal_immutable();

CREATE TRIGGER trg_card_status_history_immutable
    BEFORE UPDATE OR DELETE ON card_status_history
    FOR EACH ROW EXECUTE FUNCTION fn_trg_journal_immutable();

-- Тригерні функції викликаються лише тригерами; право PUBLIC на них не потрібне.
REVOKE EXECUTE ON ALL ROUTINES IN SCHEMA transit FROM PUBLIC;
