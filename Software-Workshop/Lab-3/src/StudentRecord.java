public class StudentRecord {
    private final String lastName;
    private final int recordNumber;
    private final int course;
    private final double averageGrade;

    // Constructor
    public StudentRecord(String lastName, int recordNumber, int course, double averageGrade) {
        this.lastName = lastName;
        this.recordNumber = recordNumber;
        this.course = course;
        this.averageGrade = averageGrade;
    }

    // Getters for filtering
    public int getCourse() { return course; }
    public double getAverageGrade() { return averageGrade; }

    // Static method to print the table header
    public static void printTableHeader() {
        System.out.println("-".repeat(66));
        System.out.printf("| %-15s | %-16s | %-6s | %-13s |%n",
                "Прізвище", "Номер заліковки", "Курс", "Середній бал");
        System.out.println("-".repeat(66));
    }

    // Method to print the object's data as a table row
    public void printTableRow() {
        System.out.printf("| %-15s | %-16d | %-6d | %-13.2f |%n",
                lastName, recordNumber, course, averageGrade);
    }
}