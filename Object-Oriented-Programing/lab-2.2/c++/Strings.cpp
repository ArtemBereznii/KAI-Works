#include "Strings.h"
#include <algorithm>

// Main class Strings

Strings::Strings() : value("") {}

Strings::Strings(const std::string& val) : value(val) {}

Strings::Strings(const Strings& other) : value(other.value) {}

const std::string& Strings::getValue() const {
    return value;
}

size_t Strings::calculateLength() const {
    return value.length();
}

std::string Strings::sortAndReturn() const {
    std::string sorted = value;
    std::sort(sorted.begin(), sorted.end());
    return sorted;
}

// Additional class UppercaseLetters

UppercaseLetters::UppercaseLetters() : Strings() {}

UppercaseLetters::UppercaseLetters(const std::string& val) : Strings(val) {}

UppercaseLetters::UppercaseLetters(const UppercaseLetters& other) : Strings(other) {}

std::string UppercaseLetters::sortAndReturn() const {
    std::string sorted = value;
    std::sort(sorted.begin(), sorted.end());
    return sorted;
}

// Additional class LowercaseLetters

LowercaseLetters::LowercaseLetters() : Strings() {}

LowercaseLetters::LowercaseLetters(const std::string& val) : Strings(val) {}

LowercaseLetters::LowercaseLetters(const LowercaseLetters& other) : Strings(other) {}

std::string LowercaseLetters::sortAndReturn() const {
    std::string sorted = value;
    std::sort(sorted.rbegin(), sorted.rend());
    return sorted;
}