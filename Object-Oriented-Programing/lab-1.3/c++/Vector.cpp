#include "Vector.h"

#include <iostream>
#include <cstdarg>

const double M_PI = 3.14159265358979323846;

Vector::Vector() : length(0), angle(0) {}

Vector::Vector(double l, double a) : length(l), angle(a) {}

Vector::Vector(const Vector& other) : length(other.length), angle(other.angle) {}

void Vector::rotate(double angle) {
    this->angle += angle;
}

void Vector::rotate() {
    this->angle += M_PI / 2;
}

double Vector::getLength() const {
    return length;
}

double Vector::getAngle() const {
    return angle;
}

Vector Vector::operator+(const Vector& other) const {
    double x1 = length * cos(angle);
    double y1 = length * sin(angle);

    double x2 = other.length * cos(other.angle);
    double y2 = other.length * sin(other.angle);

    double x = x1 + x2;
    double y = y1 + y2;

    double newLength = sqrt(x * x + y * y);
    double newAngle = atan2(y, x);

    return Vector(newLength, newAngle);
}

Vector Vector::operator*(double scalar) const {
    return Vector(length * scalar, angle);
}

Vector Vector::AddVectors(int count, ...) {
    va_list args;
    va_start(args, count);

    Vector result;

    for (int i = 0; i < count; ++i) {
        Vector v = va_arg(args, Vector);
        result = result + v;
    }

    va_end(args);
    return result;
}

void Vector::print() const {
    std::cout << "Length: " << length << ", Angle: " << angle << " radians" << std::endl;
}