using System;
using ExpressionCalculatorLibrary;

class Program
{
    static void DemonstrateExceptionHandling()
    {
        try
        {
            var calc = new ExpressionCalculator(-1, 1, 1);
            calc.CalculateExpression();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Demonstration error: {ex.Message}");
        }
    }

    static void Main()
    {
        var calculators = new ExpressionCalculator[]
        {
            new ExpressionCalculator(4, 2, 3),
            new ExpressionCalculator(1, 5, 2),
            new ExpressionCalculator(0, 3, 4) // Will cause division by zero
        };

        try
        {
            for (int i = 0; i < calculators.Length; i++)
            {
                try
                {
                    double result = calculators[i].CalculateExpression();
                    Console.WriteLine($"Calculator {i} result: {result:F4}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in calculator {i}: {ex.Message}");
                }
            }

            DemonstrateExceptionHandling();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fatal error: {ex.Message}");
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}