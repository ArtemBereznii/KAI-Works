#ifndef EXPRESSION_CALCULATOR_H
#define EXPRESSION_CALCULATOR_H

#include <cmath>
#include <stdexcept>

class ExpressionCalculator {
private:
    double a;
    double c;
    double d;

public:
    ExpressionCalculator(double a, double c, double d);

    void setA(double value);
    void setC(double value);
    void setD(double value);

    double getA() const;
    double getC() const;
    double getD() const;

    double calculateSquareRoot() const;
    double calculateExpression() const;
};

#endif