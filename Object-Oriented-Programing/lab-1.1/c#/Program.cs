using System;

class Program
{
    static void Main(string[] args)
    {
        Console.Write("Введіть рядок: ");
        string input = Console.ReadLine();

        StringManipulator sm = new StringManipulator(input);

        Console.WriteLine("Початковий рядок: " + sm.GetText());
        Console.WriteLine("Довжина рядка: " + sm.CalculateLength());

        sm.ShiftRight();
        Console.WriteLine("Рядок після зсуву праворуч: " + sm.GetText());
    }
}