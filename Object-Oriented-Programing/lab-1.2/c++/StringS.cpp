#include "Vector.h"
#include <cmath>

// Конструктор за замовчуванням
Vector2D::Vector2D() : length(0), angle(0) {}

// Конструктор з параметрами
Vector2D::Vector2D(double l, double a) : length(l), angle(a) {}

// Конструктор копіювання
Vector2D::Vector2D(const Vector2D& other) : length(other.length), angle(other.angle) {}

// Поворот вектора на заданий кут
void Vector2D::rotate(double angle) {
    this->angle += angle;
}

// Отримання довжини вектора
double Vector2D::getLength() const {
    return length;
}

// Отримання кута вектора
double Vector2D::getAngle() const {
    return angle;
}

// Перевантаження оператора додавання
Vector2D Vector2D::operator+(const Vector2D& other) const {
    double x1 = length * cos(angle);
    double y1 = length * sin(angle);

    double x2 = other.length * cos(other.angle);
    double y2 = other.length * sin(other.angle);

    double x = x1 + x2;
    double y = y1 + y2;

    double newLength = sqrt(x * x + y * y);
    double newAngle = atan2(y, x);

    return Vector2D(newLength, newAngle);
}

// Перевантаження оператора множення на скаляр
Vector2D Vector2D::operator*(double scalar) const {
    return Vector2D(length * scalar, angle);
}

// Виведення вектора на екран
void Vector2D::print() const {
    std::cout << "Length: " << length << ", Angle: " << angle << " radians" << std::endl;
}