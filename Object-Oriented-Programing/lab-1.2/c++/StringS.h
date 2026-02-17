#ifndef VECTOR_H
#define VECTOR_H

#include <iostream>
#include <cmath>

class Vector2D {
private:
    double length; // Довжина вектора
    double angle;  // Кут вектора (в радіанах)

public:
    // Конструктори
    Vector2D(); // Конструктор за замовчуванням
    Vector2D(double l, double a); // Конструктор з параметрами
    Vector2D(const Vector2D& other); // Конструктор копіювання

    // Методи
    void rotate(double angle); // Поворот вектора на заданий кут
    double getLength() const; // Отримання довжини вектора
    double getAngle() const; // Отримання кута вектора

    // Перевантаження операторів
    Vector2D operator+(const Vector2D& other) const; // Додавання векторів
    Vector2D operator*(double scalar) const; // Множення вектора на скаляр

    // Виведення вектора на екран
    void print() const;
};

#endif