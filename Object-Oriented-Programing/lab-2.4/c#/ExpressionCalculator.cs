using System;

namespace ExpressionCalculatorLibrary
{
    public class ExpressionCalculator
    {
        private double a;
        private double c;
        private double d;

        public ExpressionCalculator(double a, double c, double d)
        {
            this.a = a;
            this.c = c;
            this.d = d;
        }

        public double A
        {
            get => a;
            set => a = value;
        }

        public double C
        {
            get => c;
            set => c = value;
        }

        public double D
        {
            get => d;
            set => d = value;
        }

        private double CalculateSquareRoot()
        {
            double expression = 23 * a;
            if (expression < 0)
            {
                throw new ArgumentException("Square root of negative number");
            }
            return Math.Sqrt(expression);
        }

        public double CalculateExpression()
        {
            try
            {
                double denominator = (a / 4.0) - 1.0;
                if (Math.Abs(denominator) < 1e-10)
                {
                    throw new DivideByZeroException("Division by zero");
                }

                double sqrtValue = CalculateSquareRoot();
                return (2 * c - d + sqrtValue) / denominator;
            }
            catch (ArgumentException ex)
            {
                throw new InvalidOperationException("Invalid expression parameters", ex);
            }
        }
    }
}