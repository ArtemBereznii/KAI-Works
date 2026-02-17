#include "Segment.h"
#include <cmath>

Segment::Segment() : Line() {}

Segment::Segment(double x1, double y1, double x2, double y2)
    : Line(x1, y1, x2, y2) {
}

Segment::Segment(const Segment& other) : Line(other) {}

double Segment::calculateAngleWithOY() const {
    double dx = x2 - x1;
    double dy = y2 - y1;
    return atan2(dx, dy) * 180 / 3.14;
}