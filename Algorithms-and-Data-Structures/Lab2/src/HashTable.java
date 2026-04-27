public class HashTable {
    private final Triangle[] table;
    private final int size;

    public HashTable(int size) {
        this.size = size;
        this.table = new Triangle[size];
    }

    // Hashing method: Division
    private int hash(double key) {
        return Math.abs((int) Math.round(key)) % size;
    }

    public boolean insert(Triangle t) {
        int index = hash(t.getPerimeter());
        if (table[index] == null) {
            table[index] = t;
            return true;
        }
        return false;
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