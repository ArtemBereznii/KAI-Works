import java.util.Scanner;

public class Main {
    public static void main(String[] args) {
        Scanner scanner = new Scanner(System.in);
        System.out.print("Введіть розмір хеш-таблиці: ");
        int size = scanner.nextInt();

        /* ================= LEVEL 1 ================= */
        HashTable ht1 = new HashTable(size);
        int insertedLevel1 = 0;

        // Loop until we successfully insert enough elements without collisions
        while (insertedLevel1 < size / 2) {
            Triangle t = new Triangle();
            if (ht1.insert(t)) {
                insertedLevel1++;
            }
        }
        System.out.println("\n=== ЗАВДАННЯ ПЕРШОГО РІВНЯ ===");
        ht1.printTable();


        /* ================= LEVEL 2 ================= */
        HashTableChaining htChaining = new HashTableChaining(size);

        // Insert double the amount of the size to guarantee collisions
        int elementsToInsert = size * 2;
        for (int i = 0; i < elementsToInsert; i++) {
            htChaining.insert(new Triangle());
        }
        System.out.println("\n=== ЗАВДАННЯ ДРУГОГО РІВНЯ ===");
        htChaining.printTable();


        /* ================= LEVEL 3 ================= */
        System.out.println("\n=== ЗАВДАННЯ ТРЕТЬОГО РІВНЯ ===");
        System.out.print("Введіть мінімальну площу для видалення (наприклад, 20,5): ");
        double minArea = scanner.nextDouble();

        htChaining.deleteByMinArea(minArea);

        System.out.println("\n--- Таблиця після видалення (S < " + minArea + ") ---");
        htChaining.printTable();

        scanner.close();
    }
}