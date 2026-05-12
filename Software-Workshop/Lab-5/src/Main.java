import java.time.LocalDate;
import java.util.ArrayList;
import java.util.List;

@SuppressWarnings("unused")
interface InformationSearchable {
    void searchRecordsByDate(LocalDate date);
}

class Notepad implements InformationSearchable {
    private final String notepadName;
    private final List<RecordByDate> records;

    public Notepad(String notepadName) {
        this.notepadName = notepadName;
        this.records = new ArrayList<>();
    }

    public void addRecord(LocalDate date, String description) {
        RecordByDate newRecord = new RecordByDate(date, description);
        records.add(newRecord);
        System.out.println("Додано запис на " + date + ": " + newRecord.getDescription());
    }

    static class RecordByDate {
        private final LocalDate date;
        private final String description;

        public RecordByDate(LocalDate date, String description) {
            this.date = date;
            this.description = description;
        }

        public LocalDate getDate() {
            return date;
        }

        public String getDescription() {
            return description;
        }
    }

    @Override
    public void searchRecordsByDate(LocalDate searchDate) {
        System.out.println("\n--- Пошук записів на дату: " + searchDate + " у блокноті '" + notepadName + "' ---");
        boolean isFound = false;

        for (RecordByDate record : records) {
            if (record.getDate().equals(searchDate)) {
                System.out.println("- " + record.getDescription());
                isFound = true;
            }
        }

        if (!isFound) {
            System.out.println("Записів на цю дату не знайдено.");
        }
    }
}

public class Main {
    public static void main(String[] args) {
        Notepad myNotepad = new Notepad("Робочі завдання");

        myNotepad.addRecord(LocalDate.of(2026, 5, 12), "Синхронізація баз даних проекту");
        myNotepad.addRecord(LocalDate.of(2026, 5, 12), "Код-рев'ю з Кирилом");
        myNotepad.addRecord(LocalDate.of(2026, 5, 15), "Завершення розробки бекенду");

        myNotepad.searchRecordsByDate(LocalDate.of(2026, 5, 12));

        myNotepad.searchRecordsByDate(LocalDate.of(2026, 5, 13));
    }
}