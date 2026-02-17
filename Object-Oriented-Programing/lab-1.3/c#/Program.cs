using System;

class Program
{
    static void Main(string[] args)
    {
        Vector Z1 = new Vector();
        Vector Z2 = new Vector(3.0, 0.0);
        Vector Z3 = new Vector(Z2);

        Console.WriteLine("Initial vectors:");
        Console.Write("Z1: "); Z1.Print();
        Console.Write("Z2: "); Z2.Print();
        Console.Write("Z3: "); Z3.Print();

        Z3 = Z3 * 2;
        Console.WriteLine("\nAfter scaling Z3 by 2:");
        Console.Write("Z3: "); Z3.Print();

        Z3.Rotate();
        Console.WriteLine("\nAfter rotating Z3 by 90 degrees:");
        Console.Write("Z3: "); Z3.Print();

        Z1 = Z2 + Z3;
        Console.WriteLine("\nAfter adding Z2 and Z3 (result in Z1):");
        Console.Write("Z1: "); Z1.Print();

        Vector Z4 = new Vector(1.0, Math.PI / 4);
        Vector result = Vector.AddVectors(Z1, Z2, Z4);
        Console.WriteLine("\nAfter adding multiple vectors:");
        Console.Write("Result: "); result.Print();

        Z3.Rotate(angle: Math.PI / 4);
        Console.WriteLine("\nAfter rotating Z3 by 45 degrees using named arguments:");
        Console.Write("Z3: "); Z3.Print();
    }
}