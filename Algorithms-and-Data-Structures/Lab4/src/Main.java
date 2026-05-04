import java.util.Arrays;

public class Main {

    public static void main(String[] args) {
        // Початкові дані для тестування
        Student[] initialStudents = {
                new Student("Шевченко", "Тарас", "КН-1", 4.5, 12),
                new Student("Франко", "Іван", "КН-1", 4.8, 2),
                new Student("Українка", "Леся", "КН-2", 5.0, 0),
                new Student("Коцюбинський", "Михайло", "КН-2", 4.5, 5),
                new Student("Нечуй-Левицький", "Іван", "КН-3", 3.7, 20),
                new Student("Стус", "Василь", "КН-3", 4.8, 0)
        };

        System.out.println("============= ЗАВДАННЯ ПЕРШОГО РІВНЯ =============");
        Student[] arrayForLevel1 = Arrays.copyOf(initialStudents, initialStudents.length);
        System.out.println("Масив до сортування:");
        printArrayByMissedClasses(arrayForLevel1);

        shellSort(arrayForLevel1);

        System.out.println("\nМасив після сортування Шелла (Зростання пропусків):");
        printArrayByMissedClasses(arrayForLevel1);


        System.out.println("\n============= ЗАВДАННЯ ДРУГОГО РІВНЯ =============");
        SinglyLinkedList listForLevel2 = new SinglyLinkedList();
        for (Student s : initialStudents) {
            listForLevel2.add(s);
        }

        System.out.println("Список до сортування:");
        listForLevel2.printList();

        listForLevel2.bubbleSort();

        System.out.println("\nСписок після сортування Бульбашкою (Спадання сер.балу -> Зростання пропусків):");
        listForLevel2.printList();


        System.out.println("\n============= ЗАВДАННЯ ТРЕТЬОГО РІВНЯ =============");
        Student[] arrayForLevel3 = Arrays.copyOf(initialStudents, initialStudents.length);
        System.out.println("Масив до сортування:");
        printArrayByMissedClasses(arrayForLevel3);

        mergeSort(arrayForLevel3, 0, arrayForLevel3.length - 1);

        System.out.println("\nМасив після сортування Низхідним злиттям (Зростання пропусків):");
        printArrayByMissedClasses(arrayForLevel3);
    }

    // Рівень 1: Сортування Шелла (за зростанням кількості пропущених занять)
    public static void shellSort(Student[] arr) {
        int n = arr.length;
        for (int gap = n / 2; gap > 0; gap /= 2) {
            for (int i = gap; i < n; i++) {
                Student temp = arr[i];
                int j;
                for (j = i; j >= gap && arr[j - gap].getMissedClasses() > temp.getMissedClasses(); j -= gap) {
                    arr[j] = arr[j - gap];
                }
                arr[j] = temp;
            }
        }
    }

    // Рівень 3: Сортування низхідного злиття
    public static void mergeSort(Student[] arr, int left, int right) {
        if (left < right) {
            int mid = left + (right - left) / 2;
            mergeSort(arr, left, mid);
            mergeSort(arr, mid + 1, right);
            merge(arr, left, mid, right);
        }
    }

    private static void merge(Student[] arr, int left, int mid, int right) {
        int n1 = mid - left + 1;
        int n2 = right - mid;

        Student[] L = new Student[n1];
        Student[] R = new Student[n2];

        System.arraycopy(arr, left, L, 0, n1);
        System.arraycopy(arr, mid + 1, R, 0, n2);

        int i = 0, j = 0;
        int k = left;

        while (i < n1 && j < n2) {
            if (L[i].getMissedClasses() <= R[j].getMissedClasses()) {
                arr[k] = L[i];
                i++;
            } else {
                arr[k] = R[j];
                j++;
            }
            k++;
        }

        while (i < n1) {
            arr[k] = L[i];
            i++; k++;
        }

        while (j < n2) {
            arr[k] = R[j];
            j++; k++;
        }
    }

    public static void printArrayByMissedClasses(Student[] arr) {
        for (Student s : arr) {
            System.out.println(s.toStringByMissedClasses());
        }
    }
}