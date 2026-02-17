#ifndef SEGMENT_H
#define SEGMENT_H

#include "Line.h"

class Segment : public Line {
public:
    Segment();
    Segment(double x1, double y1, double x2, double y2);
    Segment(const Segment& other);
    double calculateAngleWithOY() const;
};

#endif