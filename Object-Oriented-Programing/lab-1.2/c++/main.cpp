#include <iostream>
#include "StringS.h"
using namespace std;

int main() {
    StringManipulator sm1;
    cout << "Default string: " << sm1.getText() << endl;

    string input;
    cout << "Enter a string: ";
    getline(cin, input);
    StringManipulator sm2(input);
    cout << "Entered string: " << sm2.getText() << endl;
    cout << "Length of the string: " << sm2.calculateLength() << endl;

    StringManipulator sm3 = sm2;
    cout << "String after copying: " << sm3.getText() << endl;

    sm3.shiftRight();
    cout << "String after shifting right: " << sm3.getText() << endl;

    sm1 = sm3;
    cout << "String after assignment: " << sm1.getText() << endl;

    return 0;
}