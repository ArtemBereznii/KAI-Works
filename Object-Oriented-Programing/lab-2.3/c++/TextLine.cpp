#include "TextLine.h"

TextLine::TextLine(const std::string& line) : content(line) {}

const std::string& TextLine::getContent() const {
    return content;
}

std::string TextLine::getDigitsString() const {
    std::string digits;
    for (char c : content) {
        if (isdigit(c)) {
            digits += c;
        }
    }
    return digits;
}