package models;

public class Node {
    Integer data;
    Node prev;
    Node next;

    public Node(Integer data) {
        this.data = data;
        this.prev = null;
        this.next = null;
    }
}
