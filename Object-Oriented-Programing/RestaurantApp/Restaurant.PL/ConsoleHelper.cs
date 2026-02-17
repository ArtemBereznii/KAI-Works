using System;

namespace Restaurant.PL
{
    public static class ConsoleHelpers
    {
        // Читає непустий рядок
        public static string ReadString(string prompt)
        {
            string result;
            do
            {
                Console.Write($"{prompt}: ");
                result = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(result))
                {
                    Console.WriteLine("Поле не може бути порожнім. Спробуйте ще раз.");
                }
            } while (string.IsNullOrWhiteSpace(result));
            return result;
        }

        // Читає ціле число з валідацією (включно з діапазоном)
        public static int ReadInt(string prompt, int min = int.MinValue, int max = int.MaxValue)
        {
            int result;
            bool isValid;
            do
            {
                Console.Write($"{prompt} ({min}-{max}): ");
                isValid = int.TryParse(Console.ReadLine(), out result);
                if (!isValid || result < min || result > max)
                {
                    Console.WriteLine($"Некоректне значення. Введіть число від {min} до {max}.");
                    isValid = false;
                }
            } while (!isValid);
            return result;
        }

        // Читає дробове число (ціну)
        public static decimal ReadDecimal(string prompt, decimal min = 0)
        {
            decimal result;
            bool isValid;
            do
            {
                Console.Write($"{prompt} (мін. {min}): ");
                isValid = decimal.TryParse(Console.ReadLine(), out result);
                if (!isValid || result < min)
                {
                    Console.WriteLine($"Некоректне значення. Введіть число більше або рівне {min}.");
                    isValid = false;
                }
            } while (!isValid);
            return result;
        }

        // Читає час приготування (у хвилинах)
        public static TimeSpan ReadTimeSpan(string prompt)
        {
            int minutes;
            bool isValid;
            do
            {
                Console.Write($"{prompt} (у хвилинах): ");
                isValid = int.TryParse(Console.ReadLine(), out minutes);
                if (!isValid || minutes <= 0)
                {
                    Console.WriteLine("Некоректне значення. Введіть додатне число хвилин.");
                    isValid = false;
                }
            } while (!isValid);
            return TimeSpan.FromMinutes(minutes);
        }

        // Пауза, щоб користувач встиг прочитати повідомлення
        public static void PressEnterToContinue()
        {
            Console.WriteLine("\nНатисніть Enter, щоб продовжити...");
            Console.ReadLine();
        }
    }
}