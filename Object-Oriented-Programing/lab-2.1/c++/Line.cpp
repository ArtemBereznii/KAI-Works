#include "Line.h"
#include <cmath>

Line::Line() : x1(0), y1(0), x2(0), y2(0) {}

Line::Line(double x1, double y1, double x2, double y2)
    : x1(x1), y1(y1), x2(x2), y2(y2) {
}

Line::Line(const Line& other)
    : x1(other.x1), y1(other.y1), x2(other.x2), y2(other.y2) {
}

double Line::calculateLength() const {
    return sqrt(pow(x2 - x1, 2) + pow(y2 - y1, 2));
}

double Line::getX1() const { return x1; }
double Line::getY1() const { return y1; }
double Line::getX2() const { return x2; }
double Line::getY2() const { return y2; }