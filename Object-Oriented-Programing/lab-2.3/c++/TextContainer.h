#pragma once
#include "TextLine.h"
#include <vector>

class TextContainer {
private:
    std::vector<TextLine> lines;

public:
    void addLine(const TextLine& line);
    void removeLine(size_t index);
    void replaceLine(size_t index, const TextLine& newLine);
    void clearText();
    size_t getLinesCount() const;
    std::string getAllDigits() const;
};