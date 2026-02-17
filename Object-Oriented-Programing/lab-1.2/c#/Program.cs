using System;

class Program
{
    static void Main(string[] args)
    {
        StringManipulator sm1 = new StringManipulator();
        Console.WriteLine("Default string: " + sm1.GetText());

        Console.Write("Enter a string: ");
        string input = Console.ReadLine();

        StringManipulator sm2 = new StringManipulator(input);
        Console.WriteLine("Entered string: " + sm2.GetText());
        Console.WriteLine("Length of the string: " + sm2.CalculateLength());

        StringManipulator sm3 = new StringManipulator(sm2);
        Console.WriteLine("String after copying: " + sm3.GetText());

        sm3.ShiftRight();
        Console.WriteLine("String after shifting right: " + sm3.GetText());

        sm1 = sm3;
        Console.WriteLine("String after assignment: " + sm1.GetText());
    }
}