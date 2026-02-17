#include "ExpressionCalculator.h"

ExpressionCalculator::ExpressionCalculator(double a, double c, double d)
    : a(a), c(c), d(d) {
}

void ExpressionCalculator::setA(double value) { a = value; }
void ExpressionCalculator::setC(double value) { c = value; }
void ExpressionCalculator::setD(double value) { d = value; }

double ExpressionCalculator::getA() const { return a; }
double ExpressionCalculator::getC() const { return c; }
double ExpressionCalculator::getD() const { return d; }

double ExpressionCalculator::calculateSquareRoot() const {
    double expression = 23 * a;
    if (expression < 0) {
        throw std::invalid_argument("Square root of negative number");
    }
    return sqrt(expression);
}

double ExpressionCalculator::calculateExpression() const {
    try {
        double denominator = (a / 4.0) - 1.0;
        if (fabs(denominator) < 1e-10) {
            throw std::runtime_error("Division by zero");
        }

        double sqrtValue = calculateSquareRoot();
        return (2 * c - d + sqrtValue) / denominator;
    }
    catch (const std::invalid_argument& e) {
        throw; // Re-throw the exception
    }
}