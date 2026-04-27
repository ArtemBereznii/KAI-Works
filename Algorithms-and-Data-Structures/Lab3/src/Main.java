import java.util.List;

public class Main {
    public static void main(String[] args) {
        BinaryTree tree = new BinaryTree();
        String universityCity = "Київ"; // Базове місто для визначення "інших міст"

        System.out.println("=== РІВЕНЬ 1: СТВОРЕННЯ ДЕРЕВА ===");

        // Корінь
        tree.insert(new Student(50, "Євтушенко", "Влад", 2, "Київ"));

        // Ліва гілка ID 30)
        tree.insert(new Student(30, "Шо", "Артем", 1, "Черкаси"));
        tree.insert(new Student(20, "Коваленко", "Іван", 2, "Київ"));
        tree.insert(new Student(40, "Бондар", "Олег", 2, "Київ"));

        // Лист
        tree.insert(new Student(10, "Мельник", "Кіріл", 1, "Покровськ"));

        // Права гілка ID 70
        tree.insert(new Student(70, "Бойко", "Максим", 1, "Харків"));
        tree.insert(new Student(80, "Шевченко", "Тарас", 2, "Київ"));

        tree.printTable();

        System.out.println("\n=== РІВЕНЬ 2: ПОШУК ===");
        System.out.println("Критерій: Студенти 1-го курсу, які прибули з міст, окрім '" + universityCity + "'");

        List<Student> searchResults = tree.searchTargetStudents(universityCity);

        if (!searchResults.isEmpty()) {
            String separator = "-".repeat(67);
            System.out.println(separator);
            for (Student student : searchResults) {
                System.out.println(student.toString());
            }
            System.out.println(separator);
        } else {
            System.out.println("Студентів за вказаним критерієм не знайдено.");
        }

        System.out.println("\n=== РІВЕНЬ 3: ВИДАЛЕННЯ ===");
        System.out.println("Демонстрація видалення вузлів з 0, 1 та 2 нащадками:");
        tree.deleteTargetNodes(universityCity);

        System.out.println("Кінцевий стан дерева після всіх видалень:");
        tree.printTable();
    }
}