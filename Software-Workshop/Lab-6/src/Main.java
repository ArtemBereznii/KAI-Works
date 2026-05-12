import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.TreeSet;

public class Main {
    public static void main(String[] args) {
        List<Integer> numbers = new ArrayList<>();

        numbers.add(45);
        numbers.add(12);
        numbers.add(89);
        numbers.add(12); // Дублікат
        numbers.add(3);
        numbers.add(45); // Дублікат

        System.out.println("--- Початковий список ---");
        displayList(numbers);

        System.out.println("\n--- Додавання нових елементів (77 та 100) ---");
        numbers.add(77);
        numbers.add(100);
        displayList(numbers);

        System.out.println("\n--- Сортування списку ---");
        Collections.sort(numbers);
        displayList(numbers);

        System.out.println("\n--- Перевірка дублікатів через TreeSet ---");
        checkDuplicatesWithTreeSet(numbers);
    }

    private static void displayList(List<Integer> list) {
        System.out.println("Елементи колекції: " + list);
    }

    private static void checkDuplicatesWithTreeSet(List<Integer> list) {
        TreeSet<Integer> uniqueSet = new TreeSet<>(list);

        if (list.size() == uniqueSet.size()) {
            System.out.println("Результат: Всі елементи в колекції унікальні. Повторів немає.");
        } else {
            System.out.println("Результат: Знайдено повторювані елементи!");
            int duplicateCount = list.size() - uniqueSet.size();
            System.out.println("Кількість зайвих (повторюваних) записів: " + duplicateCount);
            System.out.println("Колекція без дублікатів (TreeSet): " + uniqueSet);
        }
    }
}