import java.util.Scanner;

public class ComputationalAlgorithms {


    // Функція для Рівня 1: Інтеграл sqrt(1 + x^2 + sin(x))
    public static double f1(double x) {
        return Math.sqrt(1 + x * x + Math.sin(x));
    }

    // Функція для Рівня 2: Рівняння y(x) = x^2 - 2x + ln(x)
    public static double f2(double x) {
        return x * x - 2 * x + Math.log(x);
    }

    // Похідна для Рівня 2 (Метод дотичних)
    public static double df2(double x) {
        return 2 * x - 2 + 1.0 / x;
    }

    // Функція для Рівня 3: Диференціальне рівняння y' = e^x - 1
    public static double f3(double x, double y) {
        return Math.exp(x) - 1;
    }

    public static void main(String[] args) {
        Scanner scanner = new Scanner(System.in);

        System.out.println("=== РІВЕНЬ 1: Обчислення інтеграла ===");
        System.out.print("Введіть a (початок інтервалу, за завданням 0): ");
        double a1 = scanner.nextDouble();
        System.out.print("Введіть b (кінець інтервалу, за завданням 2): ");
        double b1 = scanner.nextDouble();
        System.out.print("Введіть крок інтегрування h (за завданням 0.5): ");
        double h1 = scanner.nextDouble();

        int n1 = (int) Math.round((b1 - a1) / h1);

        double rectSum = 0;
        for (int i = 0; i < n1; i++) {
            double xMid = a1 + h1 * (i + 0.5);
            rectSum += f1(xMid);
        }
        System.out.printf("Метод прямокутників: %.5f\n", h1 * rectSum);

        double trapSum = (f1(a1) + f1(b1)) / 2.0;
        for (int i = 1; i < n1; i++) {
            trapSum += f1(a1 + i * h1);
        }
        System.out.printf("Метод трапецій:      %.5f\n", h1 * trapSum);

        int nSimp = n1;
        double hSimp = h1;
        if (nSimp % 2 != 0) {
            nSimp++;
            hSimp = (b1 - a1) / nSimp;
        }
        double sum1 = 0, sum2 = 0;
        for (int i = 1; i < nSimp; i += 2) sum1 += f1(a1 + i * hSimp);
        for (int i = 2; i < nSimp; i += 2) sum2 += f1(a1 + i * hSimp);
        double simpsonResult = hSimp / 3.0 * (f1(a1) + f1(b1) + 4 * sum1 + 2 * sum2);
        System.out.printf("Метод Сімпсона:      %.5f\n", simpsonResult);

        System.out.println("\n=== РІВЕНЬ 2: Пошук коренів рівняння y(x) = 0 ===");
        System.out.println("Функція містить ln(x), тому a > 0. Рекомендований інтервал: [1, 2]");
        System.out.print("Введіть a (початок інтервалу): ");
        double a2 = scanner.nextDouble();
        System.out.print("Введіть b (кінець інтервалу): ");
        double b2 = scanner.nextDouble();
        System.out.print("Введіть точність eps (напр. 0.001): ");
        double eps = scanner.nextDouble();

        if (a2 <= 0) {
            System.out.println("Помилка: логарифм не визначений для x <= 0.");
        } else if (f2(a2) * f2(b2) > 0) {
            System.out.println("На вказаному інтервалі коренів немає, або їх парна кількість.");
        } else {
            double left = a2, right = b2, mid = 0;
            while ((right - left) > eps) {
                mid = (left + right) / 2.0;
                if (f2(a2) * f2(mid) <= 0) right = mid;
                else left = mid;
            }
            System.out.printf("Метод половинчастого ділення: x = %.5f\n", mid);

            double x0 = b2;
            double x1 = x0 - f2(x0) / df2(x0);
            while (Math.abs(x1 - x0) > eps) {
                x0 = x1;
                x1 = x0 - f2(x0) / df2(x0);
            }
            System.out.printf("Метод дотичних:               x = %.5f\n", x1);

            double x_prev = a2;
            double x_curr = b2;
            double x_next;
            do {
                x_next = x_curr - f2(x_curr) * (x_curr - x_prev) / (f2(x_curr) - f2(x_prev));
                x_prev = x_curr;
                x_curr = x_next;
            } while (Math.abs(x_curr - x_prev) > eps);
            System.out.printf("Метод хорд:                   x = %.5f\n", x_curr);
        }

        System.out.println("\n=== РІВЕНЬ 3: Диференціальне рівняння (Метод Рунге-Кутта 2-го порядку) ===");
        System.out.print("Введіть початкове значення x0: ");
        double x0_rk = scanner.nextDouble();
        System.out.print("Введіть початкове значення y0: ");
        double y0_rk = scanner.nextDouble();
        System.out.print("Введіть кінцеве значення x_end: ");
        double x_end = scanner.nextDouble();
        System.out.print("Введіть крок h: ");
        double h3 = scanner.nextDouble();

        System.out.println("\nРезультати:");
        System.out.printf("%-10s | %-10s\n", "x", "y(x)");
        System.out.println("-------------------------");

        double x = x0_rk;
        double y = y0_rk;

        System.out.printf("%-10.4f | %-10.4f\n", x, y);

        while (x < x_end - h3 / 10.0) {
            double k1 = h3 * f3(x, y);
            double k2 = h3 * f3(x + h3, y + k1);

            y = y + (k1 + k2) / 2.0;
            x = x + h3;

            System.out.printf("%-10.4f | %-10.4f\n", x, y);
        }

        scanner.close();
    }
}