import java.util.LinkedList;
import java.util.Queue;

public class RootInsertionBST {
    private RootBSTNode root;

    private RootBSTNode rotateRight(RootBSTNode currentRoot) {
        RootBSTNode newRoot = currentRoot.left;
        currentRoot.left = newRoot.right;
        newRoot.right = currentRoot;
        return newRoot;
    }

    private RootBSTNode rotateLeft(RootBSTNode currentRoot) {
        RootBSTNode newRoot = currentRoot.right;
        currentRoot.right = newRoot.left;
        newRoot.left = currentRoot;
        return newRoot;
    }

    public void insert(Student data) {
        System.out.println("Inserting at Root: " + data.lastName);
        root = insertRec(root, data);
        printBFS();
    }

    private RootBSTNode insertRec(RootBSTNode node, Student data) {
        if (node == null) {
            return new RootBSTNode(data);
        }
        int cmp = data.lastName.compareToIgnoreCase(node.data.lastName);
        if (cmp < 0) {
            node.left = insertRec(node.left, data);
            return rotateRight(node);
        } else {
            node.right = insertRec(node.right, data);
            return rotateLeft(node);
        }
    }

    @SuppressWarnings("DuplicatedCode")
    public Student search(String lastName) {
        RootBSTNode current = root;
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
        Queue<RootBSTNode> queue = new LinkedList<>();
        queue.add(root);
        System.out.println("Tree State (BFS):");
        while (!queue.isEmpty()) {
            RootBSTNode current = queue.poll();
            System.out.println(" -> " + current.data);
            if (current.left != null) queue.add(current.left);
            if (current.right != null) queue.add(current.right);
        }
        System.out.println();
    }
}