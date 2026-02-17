#pragma once
#include <string>

class IDigits {
public:
    virtual ~IDigits() = default;
    virtual std::string getDigitsString() const = 0;
};