import java.util.LinkedList;

public class HashTableChaining {
    private final LinkedList<Triangle>[] table;
    private final int size;

    @SuppressWarnings("unchecked")
    public HashTableChaining(int size) {
        this.size = size;
        // Array of linked lists for separate chaining
        table = new LinkedList[size];
        for (int i = 0; i < size; i++) {
            table[i] = new LinkedList<>();
        }
    }

    // Hashing method: Division
    private int hash(double key) {
        return Math.abs((int) Math.round(key)) % size;
    }

    // Level 2: Insert resolving collisions via separate chaining
    public boolean insert(Triangle t) {
        int index = hash(t.getPerimeter());
        table[index].add(t);
        return true;
    }

    // Level 3: Delete elements by area criterion
    public void deleteByMinArea(double minArea) {
        for (int i = 0; i < size; i++) {
            // Removes any triangle in the list where Area < minArea
            table[i].removeIf(t -> t.getArea() < minArea);
        }
    }

    public void printTable() {
        System.out.println("--- Хеш-таблиця (Роздільне зв'язування) ---");
        for (int i = 0; i < size; i++) {
            System.out.printf("Позиція %d: ", i);
            if (table[i].isEmpty()) {
                System.out.println("Порожньо");
            } else {
                for (Triangle t : table[i]) {
                    System.out.printf("[P=%.2f | %s] -> ", t.getPerimeter(), t);
                }
                System.out.println("null");
            }
        }
    }
}