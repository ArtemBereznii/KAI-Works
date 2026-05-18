import java.io.FileWriter;
import java.io.IOException;
import java.io.PrintWriter;
import java.math.BigInteger;
import java.util.Scanner;

public class CombinatoricsLab {

    public static void main(String[] args) {
        Scanner scanner = new Scanner(System.in);

        System.out.println("=== Завдання першого рівня ===");
        System.out.print("Введіть загальну кількість студентів (для варіанту 1 це 20): ");
        int totalStudents = 20;

        System.out.println("Тип вибірки: Перестановка без повторень (фіксуємо 1 елемент, переставляємо n-1).");

        BigInteger level1Result = calculateFactorial(totalStudents - 1);
        System.out.println("Кількість способів розмістити студентів: " + level1Result);
        System.out.println();

        System.out.println("=== Завдання другого рівня ===");
        System.out.print("Введіть кількість можливих букв (k-p -> 6): ");
        int lettersCount = 6;
        System.out.print("Введіть кількість можливих цифр (0-9 -> 10): ");
        int digitsCount = 10;
        System.out.print("Введіть довжину пароля (для варіанту 1 це 8): ");
        int passwordLength = 8;

        System.out.println("Тип вибірки: Розміщення з повтореннями.");

        int totalChars = lettersCount + digitsCount;
        long level2Result = lettersCount * (long) Math.pow(totalChars, passwordLength - 1);
        System.out.println("Кількість різних паролів: " + level2Result);
        System.out.println();

        System.out.println("=== Завдання третього рівня ===");
        System.out.println("Генерація перестановок для завдання першого рівня у файл 'permutations.txt'.");
        System.out.print("Увага! Для n=20 файл буде неможливо великим. Введіть тестове n (наприклад, 4), щоб перевірити роботу алгоритму: ");
        int testN = 4;


        int[] elements = new int[testN - 1];
        for (int i = 0; i < elements.length; i++) {
            elements[i] = i + 2;
        }

        try (PrintWriter writer = new PrintWriter(new FileWriter("C:\\Users\\Artem\\source\\repos\\uni\\Algorithms-and-Data-Structures\\Lab2.3\\src\\permutations.txt"))) {
            generatePermutations(elements.length, elements, writer);
            System.out.println("Усі перестановки успішно записані у файл 'permutations.txt'.");
        } catch (IOException e) {
            System.out.println("Помилка під час запису у файл: " + e.getMessage());
        }

        scanner.close();
    }


    private static BigInteger calculateFactorial(int n) {
        BigInteger fact = BigInteger.ONE;
        for (int i = 2; i <= n; i++) {
            fact = fact.multiply(BigInteger.valueOf(i));
        }
        return fact;
    }

    private static void generatePermutations(int n, int[] elements, PrintWriter writer) {
        if (n == 1) {
            writer.print("1 ");
            for (int i = 0; i < elements.length; i++) {
                writer.print(elements[i] + (i == elements.length - 1 ? "" : " "));
            }
            writer.println();
        } else {
            for (int i = 0; i < n - 1; i++) {
                generatePermutations(n - 1, elements, writer);
                if (n % 2 == 0) {
                    swap(elements, i, n - 1);
                } else {
                    swap(elements, 0, n - 1);
                }
            }
            generatePermutations(n - 1, elements, writer);
        }
    }


    private static void swap(int[] elements, int a, int b) {
        int temp = elements[a];
        elements[a] = elements[b];
        elements[b] = temp;
    }
}