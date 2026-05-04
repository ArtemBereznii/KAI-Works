import java.util.LinkedList;
import java.util.Queue;
import java.util.Scanner;

public class Task2Graph {
    public static void main(String[] args) {
        // Орієнтований граф зв'язків між школами (0 - Школа A, 1 - Школа B і т.д.)
        // 1 означає, що школа з індексом 'рядок' розсилає ПЗ школі з індексом 'стовпець'
        int[][] adjMatrix = {
                {0, 1, 0, 0, 1}, // Школа 0 відправляє 1 та 4
                {0, 0, 1, 0, 0}, // Школа 1 відправляє 2
                {1, 0, 0, 1, 0}, // Школа 2 відправляє 0 та 3
                {0, 0, 0, 0, 1}, // Школа 3 відправляє 4
                {0, 1, 0, 0, 0}  // Школа 4 відправляє 1
        };

        int n = adjMatrix.length;
        System.out.println("--- Матриця суміжності графа шкіл ---");
        for (int[] matrix : adjMatrix) {
            for (int j = 0; j < n; j++) {
                System.out.print(matrix[j] + " ");
            }
            System.out.println();
        }

        Scanner scanner = new Scanner(System.in);
        System.out.print("\nВведіть вершину, з якої почнеться обхід (від 0 до " + (n - 1) + "): ");
        int startVertex = scanner.nextInt();

        if (startVertex < 0 || startVertex >= n) {
            System.out.println("Некоректна вершина!");
            return;
        }

        System.out.print("Порядок обходу в ширину (BFS): ");
        bfs(adjMatrix, startVertex);
        System.out.println();

        scanner.close();
    }

    private static void bfs(int[][] adjMatrix, int startVertex) {
        int n = adjMatrix.length;
        boolean[] visited = new boolean[n];
        Queue<Integer> queue = new LinkedList<>();

        visited[startVertex] = true;
        queue.add(startVertex);

        while (!queue.isEmpty()) {
            int current = queue.poll();
            System.out.print(current + " ");

            for (int i = 0; i < n; i++) {
                if (adjMatrix[current][i] == 1 && !visited[i]) {
                    visited[i] = true;
                    queue.add(i);
                }
            }
        }
    }
}