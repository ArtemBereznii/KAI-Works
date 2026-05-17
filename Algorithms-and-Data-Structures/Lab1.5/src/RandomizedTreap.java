import java.util.LinkedList;
import java.util.Queue;

public class RandomizedTreap {
    private TreapNode root;

    private TreapNode rotateRight(TreapNode currentRoot) {
        TreapNode newRoot = currentRoot.left;
        currentRoot.left = newRoot.right;
        newRoot.right = currentRoot;
        return newRoot;
    }

    private TreapNode rotateLeft(TreapNode currentRoot) {
        TreapNode newRoot = currentRoot.right;
        currentRoot.right = newRoot.left;
        newRoot.left = currentRoot;
        return newRoot;
    }

    public void insert(Student data) {
        System.out.println("Inserting into Treap: " + data.lastName);
        root = insertRec(root, data);
        printBFS();
    }

    private TreapNode insertRec(TreapNode node, Student data) {
        if (node == null) {
            return new TreapNode(data);
        }
        int cmp = data.lastName.compareToIgnoreCase(node.data.lastName);
        if (cmp < 0) {
            node.left = insertRec(node.left, data);
            if (node.left.priority > node.priority) {
                node = rotateRight(node);
            }
        } else {
            node.right = insertRec(node.right, data);
            if (node.right.priority > node.priority) {
                node = rotateLeft(node);
            }
        }
        return node;
    }

    @SuppressWarnings("DuplicatedCode")
    public Student search(String lastName) {
        TreapNode current = root;
        while (current != null) {
            int cmp = lastName.compareToIgnoreCase(current.data.lastName);
            if (cmp == 0) return current.data;
            if (cmp < 0) current = current.left;
            else current = current.right;
        }
        return null;
    }

    @SuppressWarnings("DuplicatedCode")
    public void printBFS() {
        if (root == null) return;
        Queue<TreapNode> queue = new LinkedList<>();
        queue.add(root);
        System.out.println("Treap State (BFS):");
        while (!queue.isEmpty()) {
            TreapNode current = queue.poll();
            System.out.printf(" -> %s [Priority: %d]%n", current.data.lastName, current.priority);
            if (current.left != null) queue.add(current.left);
            if (current.right != null) queue.add(current.right);
        }
        System.out.println();
    }
}