import java.util.Random;

public class Triangle {
    private int x1, y1, x2, y2, x3, y3;
    private double a, b, c;

    // Constructor generates a random, valid triangle
    public Triangle() {
        generateRandom();
    }

    private void generateRandom() {
        Random rand = new Random();
        do {
            x1 = rand.nextInt(20); y1 = rand.nextInt(20);
            x2 = rand.nextInt(20); y2 = rand.nextInt(20);
            x3 = rand.nextInt(20); y3 = rand.nextInt(20);
            calculateSides();
        } while (!isValid());
    }

    private void calculateSides() {
        a = Math.hypot(x2 - x1, y2 - y1);
        b = Math.hypot(x3 - x2, y3 - y2);
        c = Math.hypot(x1 - x3, y1 - y3);
    }

    // Ensures the points form a real triangle (not a line)
    public boolean isValid() {
        return a + b > c && a + c > b && b + c > a;
    }

    // Used as the Key for the hash table (Variant 1 requirement)
    public double getPerimeter() {
        return a + b + c;
    }

    // Used for the deletion criterion in Level 3
    public double getArea() {
        double p = getPerimeter() / 2;
        return Math.sqrt(p * (p - a) * (p - b) * (p - c));
    }

    @Override
    public String toString() {
        return String.format("Трикутник[(%d,%d), (%d,%d), (%d,%d)] P=%.2f, S=%.2f",
                x1, y1, x2, y2, x3, y3, getPerimeter(), getArea());
    }
}