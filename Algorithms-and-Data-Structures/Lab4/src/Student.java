public class Student {
    private final String lastName;
    private final String firstName;
    private final String group;
    private final double averageScore;
    private final int missedClasses;

    public Student(String lastName, String firstName, String group, double averageScore, int missedClasses) {
        this.lastName = lastName;
        this.firstName = firstName;
        this.group = group;
        this.averageScore = averageScore;
        this.missedClasses = missedClasses;
    }

    public double getAverageScore() { return averageScore; }
    public int getMissedClasses() { return missedClasses; }

    // Виведення для рівнів 1 та 3 (сортування за кількістю пропусків)
    public String toStringByMissedClasses() {
        return String.format("Пропуски: %-3d | %-12s %-10s | Група: %-5s | Сер. бал: %.2f",
                missedClasses, lastName, firstName, group, averageScore);
    }

    // Виведення для рівня 2 (сортування за сер. балом, потім за пропусками)
    public String toStringByScoreAndMissed() {
        return String.format("Сер. бал: %.2f, Пропуски: %-3d | %-12s %-10s | Група: %-5s",
                averageScore, missedClasses, lastName, firstName, group);
    }
}