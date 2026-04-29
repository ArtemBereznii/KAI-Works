public class Main {
    public static void main(String[] args) {
        System.out.println("Запуск програми. Для зупинки натисніть Ctrl+C");

        // Створюємо три окремі потоки з різними цифрами та затримками (у мілісекундах)
        Thread thread1 = new Thread(new NumberTask(1, 1000));
        Thread thread2 = new Thread(new NumberTask(2, 2000));
        Thread thread3 = new Thread(new NumberTask(3, 3000));

        // Запускаємо потоки одночасно
        thread1.start();
        thread2.start();
        thread3.start();
    }
}

class NumberTask implements Runnable {
    private final int number;
    private final int delayInMillis;

    public NumberTask(int number, int delayInMillis) {
        this.number = number;
        this.delayInMillis = delayInMillis;
    }

    @Override
    public void run() {
        try {
            while (true) {
                System.out.println(number);

                Thread.sleep(delayInMillis);
            }
        } catch (InterruptedException e) {
            System.out.println("Потік для цифри " + number + " було зупинено.");
            Thread.currentThread().interrupt();
        }
    }
}