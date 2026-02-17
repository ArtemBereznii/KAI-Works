#include "TextContainer.h"

void TextContainer::addLine(const TextLine& line) {
    lines.push_back(line);
}

void TextContainer::removeLine(size_t index) {
    if (index < lines.size()) {
        lines.erase(lines.begin() + index);
    }
}

void TextContainer::replaceLine(size_t index, const TextLine& newLine) {
    if (index < lines.size()) {
        lines[index] = newLine;
    }
}

void TextContainer::clearText() {
    lines.clear();
}

size_t TextContainer::getLinesCount() const {
    return lines.size();
}

std::string TextContainer::getAllDigits() const {
    std::string allDigits;
    for (const auto& line : lines) {
        allDigits += line.getDigitsString();
    }
    return allDigits;
}