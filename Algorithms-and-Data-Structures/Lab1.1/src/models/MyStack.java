package models;

public class MyStack {
    private Integer[] array;
    private int top;

    public MyStack(int capacity) {
        this.array = new Integer[capacity];
        this.top = -1; // -1 represents an empty stack
    }

    // Mandatory check for insertion
    public boolean isFull() {
        return top == array.length - 1;
    }

    // Mandatory check for deletion
    public boolean isEmpty() {
        return top == -1;
    }

    public boolean push(Integer value) {
        if (isFull()) {
            System.out.println("Stack is full! Cannot add " + value);
            return false;
        }
        array[++top] = value;
        return true;
    }

    public Integer pop() {
        if (isEmpty()) {
            throw new RuntimeException("Stack is empty! Cannot pop.");
        }
        return array[top--]; // Return the top element, then decrement
    }

    public void printStack() {
        System.out.print("Stack (bottom to top): ");
        for (int i = 0; i <= top; i++) {
            System.out.print(array[i] + " ");
        }
        System.out.println();
    }
}
