import java.util.Scanner;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

public class Task2 {
    public static void main(String[] args) {
        String originalText = "Java - це мова програмування. Вона дозволяє створювати додатки для Android та веб-сервіси.";

        Scanner scanner = new Scanner(System.in);

        System.out.println("Вхідні параметри для зміни тексту:");
        System.out.print("Введіть рядок (суфікс), на який закінчується слово: ");
        String suffix = scanner.nextLine();

        System.out.print("Введіть слово, яке треба вставити: ");
        String wordToInsert = scanner.nextLine();

        // Розбиваємо текст на масив слів, зберігаючи пунктуацію
        String[] words = originalText.split(" ");
        StringBuilder result = new StringBuilder();

        for (int i = 0; i < words.length; i++) {
            String current = words[i];

            // group(1) - саме слово, group(2) - розділові знаки
            Pattern pattern = Pattern.compile("([a-zA-Zа-яА-ЯіїєґІЇЄҐ0-9-]+)([.!,?;:]*)");
            Matcher matcher = pattern.matcher(current);

            if (matcher.matches()) {
                String wordOnly = matcher.group(1);
                String punctuation = matcher.group(2);

                result.append(wordOnly);

                if (wordOnly.endsWith(suffix) && !suffix.isEmpty()) {
                    result.append(" ").append(wordToInsert);
                }

                result.append(punctuation);
            } else {
                result.append(current);
            }

            if (i < words.length - 1) {
                result.append(" ");
            }
        }

        // Виведення результатів
        System.out.println("\n1) Текст до обробки:");
        System.out.println(originalText);

        System.out.println("\nВхідні параметри:");
        System.out.println("Суфікс: \"" + suffix + "\", Слово для вставки: \"" + wordToInsert + "\"");

        System.out.println("\nТекст після обробки:");
        System.out.println(result);

        scanner.close();
    }
}