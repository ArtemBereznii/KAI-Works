import logic.DataConverter;
import models.MyDoublyLinkedList;
import models.MyStack;

// File: Main.java
public class Main {
    public static void main(String[] args) {

        System.out.println("=== Level 1: Creating and populating Stack ===");
        MyStack stack = new MyStack(5);
        stack.push(1);
        stack.push(2);
        stack.push(3);
        stack.push(4);
        stack.push(8);
        stack.printStack();

        System.out.println("\n=== Level 2: Initializing Doubly Linked List ===");
        MyDoublyLinkedList list = new MyDoublyLinkedList();

        System.out.println("\n=== Level 3: Transferring Data via DataConverter ===");
        // Instantiate the converter and pass the structures to it
        DataConverter converter = new DataConverter();
        converter.transferStackToList(stack, list);

        System.out.println("\n=== Final Result ===");
        list.printList();

        System.out.println("Is stack empty now? " + stack.isEmpty());
    }
}