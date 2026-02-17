using System;

public class Segment : Line
{
    public Segment() : base() { }
    public Segment(double x1, double y1, double x2, double y2) : base(x1, y1, x2, y2) { }
    public Segment(Segment other) : base(other) { }

    public double CalculateAngleWithOY()
    {
        double dx = x2 - x1;
        double dy = y2 - y1;
        return Math.Atan2(dx, dy) * (180 / Math.PI);
    }
}