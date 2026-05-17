import java.util.Arrays;
import java.util.Random;

public class AlgorithmAnalysis {

    interface Sorter {
        void sort(double[] arr);
    }

    public static void main(String[] args) {
        int N1 = 100;
        int N2 = (int) Math.pow(N1, 2);
        int N3 = (int) Math.pow(N1, 3);
        int[] sizes = {N1, N2, N3};

        int runs = 10;

        Sorter quickSort = arr -> quickSort(arr, 0, arr.length - 1);
        Sorter mergeSort = AlgorithmAnalysis::bottomUpMergeSort;

        System.out.println("=== РІВЕНЬ 1 та РІВЕНЬ 2 ===");
        System.out.println("Час виконання усереднено за " + runs + " запусків (у наносекундах).\n");
        System.out.printf("%-15s | %-20s | %-20s%n", "Розмір масиву (N)", "Швидке сортування", "Висхідне злиття");
        System.out.println("-".repeat(60));

        for (int size : sizes) {
            double[] randomArray = generateRandomArray(size);

            long timeQS = getAverageExecutionTime(quickSort, randomArray, runs);

            long timeMS = getAverageExecutionTime(mergeSort, randomArray, runs);

            System.out.printf("%-15d | %-20d | %-20d%n", size, timeQS, timeMS);
        }

        System.out.println("\n=== РІВЕНЬ 3 ===");
        System.out.println("Дослідження впливу впорядкованості на час виконання (N = 10000).\n");

        int level3Size = 10000;
        double[] randomArr = generateRandomArray(level3Size);
        double[] sortedArr = generateSortedArray(level3Size);
        double[] reverseArr = generateReverseSortedArray(level3Size);

        System.out.printf("%-20s | %-20s | %-20s%n", "Тип масиву", "Швидке сортування", "Висхідне злиття");
        System.out.println("-".repeat(65));

        long qsRandom = getAverageExecutionTime(quickSort, randomArr, runs);
        long msRandom = getAverageExecutionTime(mergeSort, randomArr, runs);
        System.out.printf("%-20s | %-20d | %-20d%n", "Випадковий (Серед.)", qsRandom, msRandom);

        long qsSorted = getAverageExecutionTime(quickSort, sortedArr, runs);
        long msSorted = getAverageExecutionTime(mergeSort, sortedArr, runs);
        System.out.printf("%-20s | %-20d | %-20d%n", "Відсортований", qsSorted, msSorted);

        long qsReverse = getAverageExecutionTime(quickSort, reverseArr, runs);
        long msReverse = getAverageExecutionTime(mergeSort, reverseArr, runs);
        System.out.printf("%-20s | %-20d | %-20d%n", "Зворотно відсорт.", qsReverse, msReverse);
    }


    private static long getAverageExecutionTime(Sorter sorter, double[] originalArray, int runs) {
        long totalTime = 0;
        for (int i = 0; i < runs; i++) {
            double[] copy = originalArray.clone();
            long startTime = System.nanoTime();
            sorter.sort(copy);
            long endTime = System.nanoTime();
            totalTime += (endTime - startTime);
        }
        return totalTime / runs;
    }

    private static double[] generateRandomArray(int size) {
        double[] arr = new double[size];
        Random rand = new Random();
        for (int i = 0; i < size; i++) {
            arr[i] = rand.nextDouble() * 1000; // Random numbers between 0 and 1000
        }
        return arr;
    }

    private static double[] generateSortedArray(int size) {
        double[] arr = generateRandomArray(size);
        Arrays.sort(arr);
        return arr;
    }

    private static double[] generateReverseSortedArray(int size) {
        double[] arr = generateSortedArray(size);
        for (int i = 0; i < size / 2; i++) {
            double temp = arr[i];
            arr[i] = arr[size - 1 - i];
            arr[size - 1 - i] = temp;
        }
        return arr;
    }

    // --- Algorithm 1: Швидке базове сортування (Basic Quick Sort) ---

    public static void quickSort(double[] arr, int low, int high) {
        if (low < high) {
            int pi = partition(arr, low, high);
            quickSort(arr, low, pi - 1);
            quickSort(arr, pi + 1, high);
        }
    }

    private static int partition(double[] arr, int low, int high) {
        double pivot = arr[high];
        int i = (low - 1);
        for (int j = low; j < high; j++) {
            if (arr[j] <= pivot) {
                i++;
                double temp = arr[i];
                arr[i] = arr[j];
                arr[j] = temp;
            }
        }
        double temp = arr[i + 1];
        arr[i + 1] = arr[high];
        arr[high] = temp;
        return i + 1;
    }

    // --- Algorithm 2: Сортування висхідним злиттям (Bottom-Up Merge Sort) ---

    public static void bottomUpMergeSort(double[] arr) {
        int n = arr.length;
        double[] temp = new double[n];

        // currSize is size of subarrays to be merged.
        for (int currSize = 1; currSize <= n - 1; currSize = 2 * currSize) {
            for (int leftStart = 0; leftStart < n - 1; leftStart += 2 * currSize) {
                int mid = Math.min(leftStart + currSize - 1, n - 1);
                int rightEnd = Math.min(leftStart + 2 * currSize - 1, n - 1);

                merge(arr, temp, leftStart, mid, rightEnd);
            }
        }
    }

    private static void merge(double[] arr, double[] temp, int left, int mid, int right) {
        int i = left;
        int j = mid + 1;
        int k = left;

        while (i <= mid && j <= right) {
            if (arr[i] <= arr[j]) {
                temp[k++] = arr[i++];
            } else {
                temp[k++] = arr[j++];
            }
        }

        while (i <= mid) {
            temp[k++] = arr[i++];
        }

        while (j <= right) {
            temp[k++] = arr[j++];
        }

        for (i = left; i <= right; i++) {
            arr[i] = temp[i];
        }
    }
}