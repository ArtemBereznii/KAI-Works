import java.util.ArrayList;
import java.util.List;

public class BinaryTree {
    private TreeNode root;

    public void insert(Student data) {
        if (root == null) {
            root = new TreeNode(data);
            return;
        }
        insertRec(root, data);
    }

    private void insertRec(TreeNode node, Student data) {
        if (data.getStudentId() == node.data.getStudentId()) {
            System.out.println("[Помилка] Студент з квитком " + data.getStudentId() + " вже існує. Ключі мають бути унікальними.");
            return;
        }

        if (data.getStudentId() < node.data.getStudentId()) {
            if (node.left == null) node.left = new TreeNode(data);
            else insertRec(node.left, data);
        } else {
            if (node.right == null) node.right = new TreeNode(data);
            else insertRec(node.right, data);
        }
    }

    public void printTable() {
        String separator = "-".repeat(67);
        System.out.println(separator);
        System.out.printf("| %-8s | %-12s | %-10s | %-4s | %-15s |\n",
                "Квиток", "Прізвище", "Ім'я", "Курс", "Місто прибуття");
        System.out.println(separator);
        printInOrder(root);
        System.out.println(separator + "\n");
    }

    private void printInOrder(TreeNode node) {
        if (node != null) {
            printInOrder(node.left);
            System.out.println(node.data.toString());
            printInOrder(node.right);
        }
    }

    // Пошук за критерієм: 1 курс, прибули з інших міст
    public List<Student> searchTargetStudents(String baseCity) {
        List<Student> results = new ArrayList<>();
        searchRec(root, baseCity, results);
        return results;
    }

    private void searchRec(TreeNode node, String baseCity, List<Student> results) {
        if (node != null) {
            searchRec(node.left, baseCity, results);

            // Перевірка критерію
            if (node.data.getCourse() == 1 && !node.data.getCityOfArrival().equalsIgnoreCase(baseCity)) {
                results.add(node.data);
            }

            searchRec(node.right, baseCity, results);
        }
    }

    // Видалення вузлів за критерієм
    public void deleteTargetNodes(String baseCity) {
        List<Student> targets = searchTargetStudents(baseCity);

        if (targets.isEmpty()) {
            System.out.println("Вузлів для видалення не знайдено.");
            return;
        }

        for (Student target : targets) {
            System.out.println("\nВидалення студента " + target.getLastName() + " (Квиток: " + target.getStudentId() + ")...");
            this.root = deleteRec(this.root, target.getStudentId());
            printTable();
        }
    }

    // Змінено назву параметра на 'node', щоб уникнути затінення змінної класу 'root'
    private TreeNode deleteRec(TreeNode node, long key) {
        if (node == null) return null;

        if (key < node.data.getStudentId()) {
            node.left = deleteRec(node.left, key);
        } else if (key > node.data.getStudentId()) {
            node.right = deleteRec(node.right, key);
        } else {
            // Вузол з 0 або 1 нащадком
            if (node.left == null) {
                return node.right;
            } else if (node.right == null) {
                return node.left;
            }

            // Вузол з 2 нащадками: отримуємо найменший елемент у правому піддереві
            node.data = minValue(node.right);

            // Видаляємо знайдений найменший елемент
            node.right = deleteRec(node.right, node.data.getStudentId());
        }
        return node;
    }

    private Student minValue(TreeNode node) {
        Student min = node.data;
        while (node.left != null) {
            min = node.left.data;
            node = node.left;
        }
        return min;
    }
}