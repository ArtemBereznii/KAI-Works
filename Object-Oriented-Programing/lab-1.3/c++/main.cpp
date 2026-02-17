#include "Vector.h"
#include <iostream>

int main() {
    const double M_PI = 3.14159265358979323846;

    Vector Z1;
    Vector Z2(3.0, 0.0);
    Vector Z3(Z2);

    std::cout << "Initial vectors:" << std::endl;
    std::cout << "Z1: "; Z1.print();
    std::cout << "Z2: "; Z2.print();
    std::cout << "Z3: "; Z3.print();

    Z3 = Z3 * 2;
    std::cout << "\nAfter scaling Z3 by 2:" << std::endl;
    std::cout << "Z3: "; Z3.print();

    Z3.rotate();
    std::cout << "\nAfter rotating Z3 by 90 degrees:" << std::endl;
    std::cout << "Z3: "; Z3.print();

    Z1 = Z2 + Z3;
    std::cout << "\nAfter adding Z2 and Z3 (result in Z1):" << std::endl;
    std::cout << "Z1: "; Z1.print();

    Vector Z4(1.0, M_PI / 4);
    Vector result = Vector::AddVectors(3, Z1, Z2, Z4);
    std::cout << "\nAfter adding multiple vectors:" << std::endl;
    std::cout << "Result: "; result.print();

    return 0;
}