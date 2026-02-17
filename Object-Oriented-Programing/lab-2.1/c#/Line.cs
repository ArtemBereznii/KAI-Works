using System;

public class Line
{
    protected double x1, y1, x2, y2;

    public Line() { x1 = y1 = x2 = y2 = 0; }  
    public Line(double x1, double y1, double x2, double y2)
    { 
        this.x1 = x1;
        this.y1 = y1;
        this.x2 = x2;
        this.y2 = y2;
    }
    public Line(Line other)
    {  
        this.x1 = other.x1;
        this.y1 = other.y1;
        this.x2 = other.x2;
        this.y2 = other.y2;
    }

    public double CalculateLength()
    {
        return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
    }

    public double X1 => x1;
    public double Y1 => y1;
    public double X2 => x2;
    public double Y2 => y2;
}