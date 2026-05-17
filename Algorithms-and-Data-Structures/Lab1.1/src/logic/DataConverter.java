package logic;

import models.MyStack;
import models.MyDoublyLinkedList;

public class DataConverter {

    // Method to handle the Level 3 assignment logic
    public void transferStackToList(MyStack stack, MyDoublyLinkedList list) {
        while (!stack.isEmpty()) {
            Integer value = stack.pop();

            if (value % 2 == 0) {
                list.addFirst(value);
                System.out.println("Popped " + value + " (Even) -> Added to START of list.");
            } else {
                list.addLast(value);
                System.out.println("Popped " + value + " (Odd)  -> Added to END of list.");
            }
        }
    }
}
