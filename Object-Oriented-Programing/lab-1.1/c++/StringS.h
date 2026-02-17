#ifndef STRINGS_H
#define STRINGS_H

#include <string>

using namespace std;

class StringManipulator {
private:
    string text;

public:
    StringManipulator(const string& str);
    int calculateLength();
    void shiftRight();
    string getText();
};

#endif