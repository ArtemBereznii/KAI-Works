import java.io.IOException;
import java.util.InputMismatchException;
import java.util.Scanner;

public class Main {
    public static void main(String[] args) {
        FileProcessor fileProcessor = new FileProcessor();

        try (Scanner scanner = new Scanner(System.in)) {
            // 1. Get file paths from the user
            System.out.print("Введіть шлях/ім'я для ВИХІДНОГО файлу (наприклад, input.txt): ");
            String inputFilePath = scanner.nextLine();

            System.out.print("Введіть шлях/ім'я для РЕЗУЛЬТУЮЧОГО файлу (наприклад, output.txt): ");
            String outputFilePath = scanner.nextLine();

            // 2. Get additional data for generation (amount of numbers)
            System.out.print("Введіть кількість випадкових чисел для генерації: ");
            int count = scanner.nextInt();

            // Defined a default range for the random numbers
            int minRange = -100;
            int maxRange = 100;

            System.out.println("\n--- Етап 1: Створення файлу ---");
            // Generate the file with random numbers
            fileProcessor.generateRandomNumbersFile(inputFilePath, count, minRange, maxRange);

            System.out.println("\n--- Етап 2: Обробка файлу ---");
            // Read, sort, and write to the new file
            fileProcessor.sortNumbersAndWriteToFile(inputFilePath, outputFilePath);

            System.out.println("\n=== Роботу програми успішно завершено ===");

        } catch (InputMismatchException e) {
            System.out.println("\n[Помилка] Некоректне введення даних з клавіатури. Очікувалося ціле число.");
        } catch (IOException e) {
            System.out.println("\n[Помилка файлової системи] Сталася помилка при роботі з файлами: " + e.getMessage());
        }
    }
}