#include <iostream>
#include "Strings.h"

void demonstratePolymorphism(const Strings& str) {
    std::cout << "Length: " << str.calculateLength() << std::endl;
    std::cout << "Sorted string: " << str.sortAndReturn() << std::endl;
}

int main() {
    UppercaseLetters upper("HELLO");
    LowercaseLetters lower("world");

    std::cout << "Original uppercase string: " << upper.getValue() << std::endl;
    demonstratePolymorphism(upper);

    std::cout << "Original lowercase string: " << lower.getValue() << std::endl;
    demonstratePolymorphism(lower);

    return 0;
}