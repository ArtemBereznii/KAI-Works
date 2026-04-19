// Завдання першого рівня: Hash table that rejects collisions
public class HashTable {
    private final Triangle[] table;
    private final int size;

    public HashTable(int size) {
        this.size = size;
        this.table = new Triangle[size];
    }

    // Hashing method: Division (Ділення)
    private int hash(double key) {
        return Math.abs((int) Math.round(key)) % size;
    }

    // Insert operation (returns false if occupied)
    public boolean insert(Triangle t) {
        int index = hash(t.getPerimeter());
        if (table[index] == null) {
            table[index] = t;
            return true;
        }
        return false; // Collision detected, insertion fails
    }

    public void printTable() {
        System.out.println("--- Хеш-таблиця (Рівень 1: Без колізій) ---");
        for (int i = 0; i < size; i++) {
            if (table[i] == null) {
                System.out.printf("Позиція %d: Порожньо%n", i);
            } else {
                System.out.printf("Позиція %d: Ключ(P)=%.2f | %s%n",
                        i, table[i].getPerimeter(), table[i].toString());
            }
        }
    }
}