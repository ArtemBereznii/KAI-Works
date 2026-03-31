public class Main {
    public static void main(String[] args) {
        System.out.println("Розробник: Березній А. А.");

        System.out.println("Знайдені числа:");

        for (int i = 1000; i <= 9999; i++) {

            if (i % 4 == 0 && i % 22 == 0) {

                // Перетворюємо число у рядковий літерал
                String numberStr = String.valueOf(i);

                int digit1 = numberStr.charAt(0) - '0';
                int digit2 = numberStr.charAt(1) - '0';
                int digit3 = numberStr.charAt(2) - '0';
                int digit4 = numberStr.charAt(3) - '0';

                if ((digit1 + digit4) == (digit2 + digit3)) {
                    System.out.println(numberStr);
                }
            }
        }
    }
}