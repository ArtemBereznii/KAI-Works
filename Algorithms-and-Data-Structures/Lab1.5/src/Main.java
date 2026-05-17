@SuppressWarnings("SpellCheckingInspection")
public class Main {
    public static void main(String[] args) {
        // Initialize the array of 20 students ordered by course
        Student[] students = {
                new Student("Shevchenko", "Taras", "IT", 1, false),
                new Student("Franko", "Ivan", "IT", 1, true),
                new Student("Ukrainka", "Lesya", "Math", 1, false),
                new Student("Kotsiubynsky", "Mykhailo", "Math", 1, true),
                new Student("Nechuy", "Ivan", "Physics", 2, false),
                new Student("Khmelnytsky", "Bohdan", "Physics", 2, true),
                new Student("Hrushevsky", "Mykhailo", "IT", 2, false),
                new Student("Mazepa", "Ivan", "History", 2, true),
                new Student("Skovoroda", "Hryhorii", "Philosophy", 3, true),  // Target Match
                new Student("Vernadsky", "Volodymyr", "Biology", 3, false),
                new Student("Sikorsky", "Ihor", "Aviation", 3, true),         // Target Match
                new Student("Korolyov", "Serhiy", "Aviation", 3, false),
                new Student("Amosov", "Mykola", "Medicine", 3, true),         // Target Match
                new Student("Paton", "Borys", "Engineering", 4, false),
                new Student("Dovzhenko", "Oleksandr", "Arts", 4, true),
                new Student("Stus", "Vasyl", "Literature", 4, false),
                new Student("Symonenko", "Vasyl", "Literature", 4, true),
                new Student("Bandera", "Stepan", "History", 5, false),
                new Student("Chornovil", "Vyacheslav", "Politics", 5, true),
                new Student("Gongadze", "Georgiy", "Journalism", 5, false)
        };

        // ================= LEVEL 1 =================
        System.out.println("================= LEVEL 1 =================");
        System.out.println("--- Array Contents ---");
        for (Student s : students) {
            System.out.println(s);
        }

        int targetCount = 0;
        for (Student student : students) {
            if (student.course == 3 && student.isContract) {
                targetCount++;
            }
        }
        System.out.println("\nResult: Total Contract Students in 3rd Course: " + targetCount);

        // ================= LEVEL 2 =================
        System.out.println("\n================= LEVEL 2 =================");
        RootInsertionBST tree = new RootInsertionBST();

        // Reusing elements from the array to populate the tree
        tree.insert(students[0]); // Shevchenko
        tree.insert(students[1]); // Franko
        tree.insert(students[2]); // Ukrainka
        tree.insert(students[5]); // Khmelnytsky

        System.out.println("--- Search Test ---");
        String searchKey1 = "Ukrainka";
        Student found1 = tree.search(searchKey1);
        System.out.println(found1 != null ? "Found Student: " + found1 : "Student '" + searchKey1 + "' not found.");

        // ================= LEVEL 3 =================
        System.out.println("\n================= LEVEL 3 =================");
        RandomizedTreap treap = new RandomizedTreap();

        // Reusing elements from the array to populate the treap
        treap.insert(students[8]);  // Skovoroda
        treap.insert(students[12]); // Amosov
        treap.insert(students[9]);  // Vernadsky
        treap.insert(students[13]); // Paton

        System.out.println("--- Search Test ---");
        String searchKey2 = "Amosov";
        Student found2 = treap.search(searchKey2);
        System.out.println(found2 != null ? "Found Student: " + found2 : "Student '" + searchKey2 + "' not found.");
    }
}