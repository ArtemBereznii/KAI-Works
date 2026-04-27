public class Student {
    private final String lastName;
    private final String firstName;
    private final int course;
    private final long studentId; // Using long to represent the unsigned integer requirement
    private final String cityOfArrival;

    public Student(long studentId, String lastName, String firstName, int course, String cityOfArrival) {
        this.studentId = studentId;
        this.lastName = lastName;
        this.firstName = firstName;
        this.course = course;
        this.cityOfArrival = cityOfArrival;
    }

    public long getStudentId() { return studentId; }
    public String getLastName() { return lastName; }
    public int getCourse() { return course; }
    public String getCityOfArrival() { return cityOfArrival; }

    @Override
    public String toString() {
        return String.format("| %-8d | %-12s | %-10s | %-4d | %-15s |",
                studentId, lastName, firstName, course, cityOfArrival);
    }
}