import java.util.Scanner;

public class Task1LUP {
    public static void main(String[] args) {
        Scanner scanner = new Scanner(System.in);
        int n = 4;

        double[][] A = new double[n][n];
        double[] b = new double[n];

        System.out.println("Введіть матрицю коефіцієнтів A (" + n + "x" + n + "):");
        /*
           5 -1 -8 -7
          -7 8 6 -5
           1 -6 3 -10
          -4 -1 -2 -5
        */
        for (int i = 0; i < n; i++) {
            for (int j = 0; j < n; j++) {
                A[i][j] = scanner.nextDouble();
            }
        }

        System.out.println("Введіть вектор вільних членів b (" + n + "):");
        /*
           140 -45 68 13
        */
        for (int i = 0; i < n; i++) {
            b[i] = scanner.nextDouble();
        }

        System.out.println("\n--- Початкова система рівнянь ---");
        printSystem(A, b);

        double[][] L = new double[n][n];
        double[][] U = new double[n][n];
        double[][] P = new double[n][n];

        for (int i = 0; i < n; i++) {
            P[i][i] = 1.0;
            System.arraycopy(A[i], 0, U[i], 0, n);
        }

        // LUP-розкладання
        for (int i = 0; i < n; i++) {
            double pivot = 0;
            int pivotRow = i;
            for (int row = i; row < n; row++) {
                if (Math.abs(U[row][i]) > pivot) {
                    pivot = Math.abs(U[row][i]);
                    pivotRow = row;
                }
            }

            // Перестановка рядків у матрицях U, P та L
            swapRows(U, i, pivotRow);
            swapRows(P, i, pivotRow);
            swapRows(L, i, pivotRow);

            L[i][i] = 1.0;

            for (int j = i + 1; j < n; j++) {
                L[j][i] = U[j][i] / U[i][i];
                for (int k = i; k < n; k++) {
                    U[j][k] -= L[j][i] * U[i][k];
                }
            }
        }

        System.out.println("\n--- Матриця P (Перестановок) ---");
        printMatrix(P);
        System.out.println("--- Матриця L (Нижньо-трикутна) ---");
        printMatrix(L);
        System.out.println("--- Матриця U (Верхньо-трикутна) ---");
        printMatrix(U);

        double[] Pb = multiply(P, b);
        double[] y = new double[n];
        for (int i = 0; i < n; i++) {
            y[i] = Pb[i];
            for (int j = 0; j < i; j++) {
                y[i] -= L[i][j] * y[j];
            }
        }

        double[] x = new double[n];
        for (int i = n - 1; i >= 0; i--) {
            x[i] = y[i];
            for (int j = i + 1; j < n; j++) {
                x[i] -= U[i][j] * x[j];
            }
            x[i] /= U[i][i];
        }

        System.out.println("\n--- Розв'язок системи ---");
        for (int i = 0; i < n; i++) {
            System.out.printf("x%d = %.4f%n", (i + 1), x[i]);
        }
        scanner.close();
    }

    private static void swapRows(double[][] matrix, int row1, int row2) {
        double[] temp = matrix[row1];
        matrix[row1] = matrix[row2];
        matrix[row2] = temp;
    }

    private static double[] multiply(double[][] P, double[] b) {
        int n = P.length;
        double[] result = new double[n];
        for (int i = 0; i < n; i++) {
            for (int j = 0; j < n; j++) {
                result[i] += P[i][j] * b[j];
            }
        }
        return result;
    }

    private static void printMatrix(double[][] matrix) {
        for (double[] row : matrix) {
            for (double val : row) {
                System.out.printf("%8.3f ", val);
            }
            System.out.println();
        }
        System.out.println();
    }

    private static void printSystem(double[][] A, double[] b) {
        for (int i = 0; i < A.length; i++) {
            for (int j = 0; j < A[i].length; j++) {
                System.out.printf("%8.3fx%d ", A[i][j], (j + 1));
                if (j < A[i].length - 1) System.out.print("+ ");
            }
            System.out.printf(" = %8.3f%n", b[i]);
        }
    }
}