import java.util.InputMismatchException;
import java.util.Scanner;

public class Main {
    public static void main(String[] args) {
        Scanner scanner = new Scanner(System.in);

        int n = readInt(scanner, "Введіть кількість студентів (не менше 5): ", 5, 100);

        StudentRecord[] students = new StudentRecord[n];

        for (int i = 0; i < n; i++) {
            System.out.println("\n--- Введення даних для студента #" + (i + 1) + " ---");
            students[i] = readStudentData(scanner);
        }

        System.out.println("\n=== Вихідні дані ===");
        printTable(students);

        // Task 1: List of students with an average grade > 4.5
        System.out.println("\n=== Результат: Студенти з середнім балом більше 4.5 ===");
        filterByGrade(students);

        // Task 2: List of students of a specified course
        System.out.println("\n=== Пошук за курсом ===");
        int searchCourse = readInt(scanner, "Введіть курс для пошуку (1-6): ", 1, 6);
        System.out.println("\n=== Результат: Студенти " + searchCourse + " курсу ===");
        filterByCourse(students, searchCourse);

        scanner.close();
    }

    private static StudentRecord readStudentData(Scanner scanner) {
        String lastName;
        while (true) {
            System.out.print("Прізвище: ");
            lastName = scanner.next();
            if (lastName.trim().isEmpty()) {
                System.out.println("Помилка: прізвище не може бути порожнім.");
                continue;
            }
            break;
        }

        int recordNumber = readInt(scanner, "Номер заліковки (ціле число): ", 1, 9999999);
        int course = readInt(scanner, "Курс (1-6): ", 1, 6);
        double averageGrade = readDouble(scanner);

        return new StudentRecord(lastName, recordNumber, course, averageGrade);
    }

    private static int readInt(Scanner scanner, String prompt, int min, int max) {
        while (true) {
            try {
                System.out.print(prompt);
                int value = scanner.nextInt();
                if (value < min || value > max) {
                    System.out.println("Помилка: значення виходить за допустимі межі (" + min + " - " + max + "). Спробуйте ще раз.");
                    continue;
                }
                return value;
            } catch (InputMismatchException e) {
                System.out.println("Помилка типу даних: очікується ціле число. Спробуйте ще раз.");
                scanner.nextLine(); // Clear the bad input buffer
            }
        }
    }

    private static double readDouble(Scanner scanner) {
        while (true) {
            try {
                System.out.print("Середній бал (0.0 - 5.0): ");
                double value = scanner.nextDouble();
                if (value < 0.0 || value > 5.0) {
                    System.out.println("Помилка: значення виходить за допустимі межі (" + 0.0 + " - " + 5.0 + "). Спробуйте ще раз.");
                    continue;
                }
                return value;
            } catch (InputMismatchException e) {
                System.out.println("Помилка типу даних: очікується дійсне число. (Увага: залежно від налаштувань ОС, можливо потрібно використовувати кому ',' замість крапки '.').");
                scanner.nextLine(); // Clear the bad input buffer
            }
        }
    }

    private static void printTable(StudentRecord[] students) {
        StudentRecord.printTableHeader();
        for (StudentRecord student : students) {
            student.printTableRow();
        }
        System.out.println("-".repeat(66));
    }

    private static void filterByGrade(StudentRecord[] students) {
        boolean found = false;
        StudentRecord.printTableHeader();
        for (StudentRecord student : students) {
            if (student.getAverageGrade() > 4.5) {
                student.printTableRow();
                found = true;
            }
        }
        System.out.println("-".repeat(66));
        if (!found) {
            System.out.println("Дані за заданим критерієм пошуку відсутні.");
        }
    }

    private static void filterByCourse(StudentRecord[] students, int course) {
        boolean found = false;
        StudentRecord.printTableHeader();
        for (StudentRecord student : students) {
            if (student.getCourse() == course) {
                student.printTableRow();
                found = true;
            }
        }
        System.out.println("-".repeat(66));
        if (!found) {
            System.out.println("Дані за заданим критерієм пошуку відсутні.");
        }
    }
}