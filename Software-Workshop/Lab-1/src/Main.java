import java.util.Random;

public class Main {
    public static void main(String[] args) {
        // Виведення ініціалів
        System.out.println("Розробник: Березній А. А.");

        // Оголошення матриці A(3x2)
        int rows = 3;
        int cols = 2;
        int[][] A = new int[rows][cols];
        Random random = new Random();

        // Формування матриці
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                // Випадкові числа від 0 до 10
                A[i][j] = random.nextInt(11);
            }
        }

        // Виведення матриці А
        System.out.println("Матриця А:");
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                System.out.print(A[i][j] + "\t");
            }
            System.out.println();
        }

        // Обчислення сум елементів по стовпцях
        int[] columnSums = new int[cols];
        for (int j = 0; j < cols; j++) {
            for (int i = 0; i < rows; i++) {
                columnSums[j] += A[i][j];
            }
        }

        // Виведення результатів
        System.out.println("\nСуми елементів матриці за стовпцями:");
        for (int j = 0; j < cols; j++) {
            System.out.println("Стовпець " + (j + 1) + ": " + columnSums[j]);
        }
    }
}