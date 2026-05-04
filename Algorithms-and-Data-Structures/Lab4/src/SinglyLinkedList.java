public class SinglyLinkedList {
    private Node head;

    private static class Node {
        Student data;
        Node next;

        Node(Student data) {
            this.data = data;
            this.next = null;
        }
    }

    public void add(Student student) {
        Node newNode = new Node(student);
        if (head == null) {
            head = newNode;
        } else {
            Node current = head;
            while (current.next != null) {
                current = current.next;
            }
            current.next = newNode;
        }
    }

    public void printList() {
        Node current = head;
        while (current != null) {
            System.out.println(current.data.toStringByScoreAndMissed());
            current = current.next;
        }
    }

    // Рівень 2: Сортування Бульбашкою
    // За спаданням середнього бала, за рівності - за зростанням кількості пропусків
    public void bubbleSort() {
        if (head == null || head.next == null) return;

        boolean swapped;
        do {
            swapped = false;
            Node current = head;

            while (current.next != null) {
                boolean shouldSwap = false;

                if (current.data.getAverageScore() < current.next.data.getAverageScore()) {
                    shouldSwap = true;
                }
                else if (current.data.getAverageScore() == current.next.data.getAverageScore()) {
                    if (current.data.getMissedClasses() > current.next.data.getMissedClasses()) {
                        shouldSwap = true;
                    }
                }

                if (shouldSwap) {
                    Student temp = current.data;
                    current.data = current.next.data;
                    current.next.data = temp;
                    swapped = true;
                }
                current = current.next;
            }
        } while (swapped);
    }
}