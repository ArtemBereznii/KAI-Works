#ifndef LINE_H
#define LINE_H

class Line {
protected:
    double x1, y1, x2, y2;

public:
    Line();
    Line(double x1, double y1, double x2, double y2); 
    Line(const Line& other); 

    double calculateLength() const;

    double getX1() const;
    double getY1() const;
    double getX2() const;
    double getY2() const;
};

#endif