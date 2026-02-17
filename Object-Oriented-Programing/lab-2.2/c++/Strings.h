#ifndef STRINGS_H
#define STRINGS_H

#include <string>
#include <algorithm>

class Strings {
protected:
    std::string value;

public:
    Strings();
    Strings(const std::string& val);
    Strings(const Strings& other);
    const std::string& getValue() const;
    virtual size_t calculateLength() const;
    virtual std::string sortAndReturn() const;
};

class UppercaseLetters : public Strings {
public:
    UppercaseLetters();
    UppercaseLetters(const std::string& val);
    UppercaseLetters(const UppercaseLetters& other);
    std::string sortAndReturn() const override;
};

class LowercaseLetters : public Strings {
public:
    LowercaseLetters();
    LowercaseLetters(const std::string& val);
    LowercaseLetters(const LowercaseLetters& other);
    std::string sortAndReturn() const override;
};

#endif