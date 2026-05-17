package models;

public class MyDoublyLinkedList {
    private Node head;
    private Node tail;

    public MyDoublyLinkedList() {
        this.head = null;
        this.tail = null;
    }

    // Mandatory check for deletion
    public boolean isEmpty() {
        return head == null;
    }

    // Adds element to the beginning (for even numbers)
    public void addFirst(Integer value) {
        Node newNode = new Node(value);
        if (isEmpty()) {
            head = tail = newNode;
        } else {
            newNode.next = head;
            head.prev = newNode;
            head = newNode;
        }
    }

    // Adds element to the end (for odd numbers)
    public void addLast(Integer value) {
        Node newNode = new Node(value);
        if (isEmpty()) {
            head = tail = newNode;
        } else {
            tail.next = newNode;
            newNode.prev = tail;
            tail = newNode;
        }
    }

    public void printList() {
        System.out.print("Doubly Linked List: ");
        Node current = head;
        while (current != null) {
            System.out.print(current.data + " <-> ");
            current = current.next;
        }
        System.out.println("null");
    }
}
