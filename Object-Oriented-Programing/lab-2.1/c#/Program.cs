using System;

class Program
{
    static void Main()
    {
        Segment segment1 = new Segment();  
        Segment segment2 = new Segment(0, 0, 3, 4);  
        Segment segment3 = new Segment(segment2);  

        Console.WriteLine($"Segment 1 (default): ({segment1.X1}, {segment1.Y1}) to ({segment1.X2}, {segment1.Y2})");
        Console.WriteLine($"Segment 2: Length = {segment2.CalculateLength()}, Angle with OY = {segment2.CalculateAngleWithOY()} degrees");
        Console.WriteLine($"Segment 3 (copy): Length = {segment3.CalculateLength()}");
    }
}