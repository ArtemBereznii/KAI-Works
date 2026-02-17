using System;
using StringsLibrary;

class Program
{
    static void DemonstratePolymorphism(Strings str)
    {
        Console.WriteLine($"Length: {str.CalculateLength()}");
        Console.WriteLine($"Sorted string: {str.SortAndReturn()}");
    }

    static void Main(string[] args)
    {
        UppercaseLetters upper = new UppercaseLetters("HELLO");
        LowercaseLetters lower = new LowercaseLetters("world");

        Console.WriteLine($"Original uppercase string: {upper.Value}");
        DemonstratePolymorphism(upper);

        Console.WriteLine($"Original lowercase string: {lower.Value}");
        DemonstratePolymorphism(lower);
    }
}