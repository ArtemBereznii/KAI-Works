using System;
using TextLibrary;

class Program
{
    static void DisplayMenu()
    {
        Console.WriteLine("\nText Container Operations:");
        Console.WriteLine("1. Add line");
        Console.WriteLine("2. Remove line");
        Console.WriteLine("3. Replace line");
        Console.WriteLine("4. Clear text");
        Console.WriteLine("5. Show line count");
        Console.WriteLine("6. Extract all digits");
        Console.WriteLine("7. Exit");
        Console.Write("Choose option: ");
    }

    static void Main()
    {
        var container = new TextContainer();
        int choice;
        string input;

        do
        {
            DisplayMenu();
            choice = int.Parse(Console.ReadLine());
            Console.ReadLine(); // Clear buffer

            switch (choice)
            {
                case 1:
                    Console.Write("Enter line to add: ");
                    input = Console.ReadLine();
                    container.AddLine(new TextLine(input));
                    Console.WriteLine("Line added successfully.");
                    break;
                case 2:
                    Console.Write("Enter line index to remove: ");
                    int index = int.Parse(Console.ReadLine());
                    container.RemoveLine(index);
                    Console.WriteLine("Line removed (if index was valid).");
                    break;
                case 3:
                    Console.Write("Enter line index to replace: ");
                    index = int.Parse(Console.ReadLine());
                    Console.ReadLine(); // Clear buffer
                    Console.Write("Enter new line content: ");
                    input = Console.ReadLine();
                    container.ReplaceLine(index, new TextLine(input));
                    Console.WriteLine("Line replaced (if index was valid).");
                    break;
                case 4:
                    container.ClearText();
                    Console.WriteLine("Text cleared.");
                    break;
                case 5:
                    Console.WriteLine($"Total lines: {container.GetLinesCount()}");
                    break;
                case 6:
                    Console.WriteLine($"All digits in text: {container.GetAllDigits()}");
                    break;
                case 7:
                    Console.WriteLine("Exiting program.");
                    break;
                default:
                    Console.WriteLine("Invalid option. Try again.");
                    break;
            }
        } while (choice != 7);
    }
}