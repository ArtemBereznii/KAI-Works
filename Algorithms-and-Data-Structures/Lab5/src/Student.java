public class Student {
    String lastName;
    String firstName;
    String faculty;
    int course;
    boolean isContract;

    public Student(String lastName, String firstName, String faculty, int course, boolean isContract) {
        this.lastName = lastName;
        this.firstName = firstName;
        this.faculty = faculty;
        this.course = course;
        this.isContract = isContract;
    }

    @Override
    public String toString() {
        return String.format("%-12s %-10s | Faculty: %-5s | Course: %d | Form: %s",
                lastName, firstName, faculty, course, (isContract ? "Contract" : "Budget"));
    }
}