#ifndef VECTOR_H
#define VECTOR_H

#include <vector>

class Vector {
private:
    double length;
    double angle;

public:
    Vector();
    Vector(double l, double a);
    Vector(const Vector& other);
    void rotate(double angle);
    void rotate();
    double getLength() const;
    double getAngle() const;
    Vector operator+(const Vector& other) const;
    Vector operator*(double scalar) const;
    static Vector AddVectors(int count, ...);
    void print() const;
};

#endif