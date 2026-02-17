using System;

public class Vector
{
    private double length;
    private double angle;

    public Vector()
    {
        length = 0;
        angle = 0;
    }

    public Vector(double l, double a)
    {
        length = l;
        angle = a;
    }

    public Vector(Vector other)
    {
        length = other.length;
        angle = other.angle;
    }

    public void Rotate(double angle)
    {
        this.angle += angle;
    }

    public void Rotate()
    {
        this.angle += Math.PI / 2;
    }

    public double GetLength()
    {
        return length;
    }

    public double GetAngle()
    {
        return angle;
    }

    public static Vector operator +(Vector v1, Vector v2)
    {
        double x1 = v1.length * Math.Cos(v1.angle);
        double y1 = v1.length * Math.Sin(v1.angle);

        double x2 = v2.length * Math.Cos(v2.angle);
        double y2 = v2.length * Math.Sin(v2.angle);

        double x = x1 + x2;
        double y = y1 + y2;

        double newLength = Math.Sqrt(x * x + y * y);
        double newAngle = Math.Atan2(y, x);

        return new Vector(newLength, newAngle);
    }

    public static Vector operator *(Vector v, double scalar)
    {
        return new Vector(v.length * scalar, v.angle);
    }

    public static Vector AddVectors(params Vector[] vectors)
    {
        Vector result = new Vector();

        foreach (var v in vectors)
        {
            result = result + v;
        }

        return result;
    }

    public void Print()
    {
        Console.WriteLine($"Length: {length}, Angle: {angle} radians");
    }
}