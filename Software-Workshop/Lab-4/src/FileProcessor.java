import java.io.*;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.Random;
import java.util.Scanner;

public class FileProcessor {

    public void generateRandomNumbersFile(String filePath, int count, int min, int max) throws IOException {
        System.out.println("-> Початок генерації випадкових чисел у файл: " + filePath);
        Random random = new Random();

        try (BufferedWriter writer = new BufferedWriter(new FileWriter(filePath))) {
            for (int i = 0; i < count; i++) {
                int randomNum = random.nextInt((max - min) + 1) + min;
                writer.write(randomNum + " ");
            }
        }
        System.out.println("-> Файл успішно створено та заповнено.");
    }

    public void sortNumbersAndWriteToFile(String inputFilePath, String outputFilePath) throws IOException {
        System.out.println("-> Читання даних з вихідного файлу: " + inputFilePath);
        List<Integer> numbers = new ArrayList<>();

        // Reading data from the input file
        try (Scanner fileScanner = new Scanner(new File(inputFilePath))) {
            while (fileScanner.hasNextInt()) {
                numbers.add(fileScanner.nextInt());
            }
        }

        if (numbers.isEmpty()) {
            System.out.println("-> Увага: Вихідний файл порожній або не містить цілих чисел.");
            return;
        }

        System.out.println("-> Прочитано чисел: " + numbers.size() + ". Виконується сортування за зростанням...");
        // Sorting the numbers
        Collections.sort(numbers);

        System.out.println("-> Запис відсортованих даних у результуючий файл: " + outputFilePath);
        // Writing the sorted numbers to the output file
        try (BufferedWriter writer = new BufferedWriter(new FileWriter(outputFilePath))) {
            for (int number : numbers) {
                writer.write(number + " ");
            }
        }
        System.out.println("-> Дані успішно відсортовано та записано.");
    }
}