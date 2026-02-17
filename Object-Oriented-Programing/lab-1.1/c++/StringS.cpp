#include "StringS.h"

using namespace std;

StringManipulator::StringManipulator(const string& str) : text(str) {}

int StringManipulator::calculateLength() {
    return text.length();
}

void StringManipulator::shiftRight() {
    if (!text.empty()) {
        char lastChar = text.back();
        text.pop_back();
        text.insert(text.begin(), lastChar);
    }
}

string StringManipulator::getText() {
    return text;
}