#include <iostream>
#include <vector>
#include "ExpressionCalculator.h"

void demonstrateExceptionHandling() {
    try {
        ExpressionCalculator calc(0, 1, 1); // Will cause division by zero
        calc.calculateExpression();
    }
    catch (const std::exception& e) {
        std::cerr << "Error in demonstration: " << e.what() << std::endl;
    }
}

int main() {
    std::vector<ExpressionCalculator> calculators;

    try {
        // Create array of objects
        calculators.emplace_back(4, 2, 3);
        calculators.emplace_back(1, 5, 2);
        calculators.emplace_back(-1, 3, 4); // Will cause exception

        // Calculate expressions
        for (size_t i = 0; i < calculators.size(); ++i) {
            try {
                double result = calculators[i].calculateExpression();
                std::cout << "Calculator " << i << " result: " << result << std::endl;
            }
            catch (const std::exception& e) {
                std::cerr << "Error in calculator " << i << ": " << e.what() << std::endl;
            }
        }

        demonstrateExceptionHandling();
    }
    catch (const std::exception& e) {
        std::cerr << "Fatal error: " << e.what() << std::endl;
        return 1;
    }

    return 0;
}