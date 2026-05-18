import java.io.BufferedReader;
import java.io.FileReader;
import java.io.IOException;
import java.util.EnumMap;
import java.util.Map;
import java.util.Scanner;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

public class IdentificationAlgorithms {

    public enum State {
        S0, S1, S2, S3, S4, S5, SUCCESS, ERROR
    }

    public enum CharType {
        PLUS, PERCENT, DIGIT, OTHER
    }

    public static void main(String[] args) {
        Scanner scanner = new Scanner(System.in);

        System.out.println("=== РІВЕНЬ 1 ===");
        level1();

        System.out.println("\n=== РІВЕНЬ 2 ===");
        level2(scanner);

        System.out.println("\n=== РІВЕНЬ 3 ===");
        level3();

        scanner.close();
    }

    public static void level1() {
        String regex = "^\\+[0-9]+\\$%\\+[0-9]+$";
        Pattern pattern = Pattern.compile(regex);

        System.out.println("Зчитування з файлу level1.txt...");
        try (BufferedReader br = new BufferedReader(new FileReader("C:\\Users\\Artem\\source\\repos\\uni\\Algorithms-and-Data-Structures\\Lab2.2\\src\\level1.txt"))) {
            String line;
            while ((line = br.readLine()) != null) {
                Matcher matcher = pattern.matcher(line.trim());
                if (matcher.matches()) {
                    System.out.println("Знайдено відповідність: " + line);
                }
            }
        } catch (IOException e) {
            System.out.println("Помилка читання файлу level1.txt: " + e.getMessage());
            System.out.println("Створіть файл level1.txt, де кожен рядок містить одне слово.");
        }
    }

    public static void level2(Scanner scanner) {
        System.out.print("Уведіть рядок для перевірки (Рівень 2): ");
        String input = scanner.nextLine();

        State state = State.S0;

        for (char c : input.toCharArray()) {
            switch (state) {
                case S0:
                    state = (c == '+') ? State.S1 : State.ERROR;
                    break;
                case S1:
                    state = Character.isDigit(c) ? State.S2 : State.ERROR;
                    break;
                case S2:
                    if (c == '+') {
                        state = State.S3;
                    } else if (!Character.isDigit(c)) {
                        state = State.ERROR;
                    }
                    break;
                case S3:
                    state = (c == '%') ? State.S4 : State.ERROR;
                    break;
                case S4:
                    state = (c == '+') ? State.S5 : State.ERROR;
                    break;
                case S5:
                case SUCCESS:
                    state = Character.isDigit(c) ? State.SUCCESS : State.ERROR;
                    break;
                case ERROR:
                    break;
            }
        }

        if (state == State.SUCCESS) {
            System.out.println("Результат: Рядок ВІДПОВІДАЄ заданій синтаксичній будові.");
        } else {
            System.out.println("Результат: Рядок НЕ ВІДПОВІДАЄ заданій синтаксичній будові.");
        }
    }

    public static void level3() {
        Map<State, Map<CharType, State>> transitionTable = buildTransitionTable();

        System.out.println("Зчитування та розбиття тексту з файлу level3.txt...");
        try (BufferedReader br = new BufferedReader(new FileReader("C:\\Users\\Artem\\source\\repos\\uni\\Algorithms-and-Data-Structures\\Lab2.2\\src\\level3.txt"))) {
            String line;
            while ((line = br.readLine()) != null) {
                String[] words = line.split("[ $#]+");

                for (String word : words) {
                    if (word.isEmpty()) continue;

                    State state = State.S0;

                    for (int i = 0; i < word.length(); i++) {
                        char c = word.charAt(i);
                        CharType type = getCharType(c);

                        Map<CharType, State> transitions = transitionTable.get(state);
                        if (transitions != null && transitions.containsKey(type)) {
                            state = transitions.get(type);
                        } else {
                            state = State.ERROR;
                            break;
                        }
                    }

                    if (state == State.SUCCESS) {
                        System.out.println("Слово '" + word + "' -> ПРАВИЛЬНЕ");
                    } else {
                        System.out.println("Слово '" + word + "' -> НЕПРАВИЛЬНЕ");
                    }
                }
            }
        } catch (IOException e) {
            System.out.println("Помилка читання файлу level3.txt: " + e.getMessage());
            System.out.println("Створіть файл level3.txt для тестування 3-го рівня.");
        }
    }

    private static CharType getCharType(char c) {
        if (c == '+') return CharType.PLUS;
        if (c == '%') return CharType.PERCENT;
        if (Character.isDigit(c)) return CharType.DIGIT;
        return CharType.OTHER;
    }

    private static Map<State, Map<CharType, State>> buildTransitionTable() {
        Map<State, Map<CharType, State>> table = new EnumMap<>(State.class);

        Map<CharType, State> s0 = new EnumMap<>(CharType.class);
        s0.put(CharType.PLUS, State.S1);
        table.put(State.S0, s0);

        Map<CharType, State> s1 = new EnumMap<>(CharType.class);
        s1.put(CharType.DIGIT, State.S2);
        table.put(State.S1, s1);

        Map<CharType, State> s2 = new EnumMap<>(CharType.class);
        s2.put(CharType.DIGIT, State.S2);
        s2.put(CharType.PLUS, State.S3);
        table.put(State.S2, s2);

        Map<CharType, State> s3 = new EnumMap<>(CharType.class);
        s3.put(CharType.PERCENT, State.S4);
        table.put(State.S3, s3);

        Map<CharType, State> s4 = new EnumMap<>(CharType.class);
        s4.put(CharType.PLUS, State.S5);
        table.put(State.S4, s4);

        Map<CharType, State> s5 = new EnumMap<>(CharType.class);
        s5.put(CharType.DIGIT, State.SUCCESS);
        table.put(State.S5, s5);

        Map<CharType, State> success = new EnumMap<>(CharType.class);
        success.put(CharType.DIGIT, State.SUCCESS);
        table.put(State.SUCCESS, success);

        return table;
    }
}