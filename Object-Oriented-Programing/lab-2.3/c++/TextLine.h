#pragma once
#include "DigitsInterface.h"
#include <string>

class TextLine : public IDigits {
private:
    std::string content;

public:
    explicit TextLine(const std::string& line);
    const std::string& getContent() const;
    std::string getDigitsString() const override;
};