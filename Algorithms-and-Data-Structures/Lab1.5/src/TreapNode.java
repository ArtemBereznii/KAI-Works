import java.util.Random;

public class TreapNode {
    Student data;
    int priority;
    TreapNode left, right;

    public TreapNode(Student data) {
        this.data = data;
        this.priority = new Random().nextInt(100000); // Random priority for balancing
    }
}