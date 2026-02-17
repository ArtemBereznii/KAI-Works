#include <iostream>
#include "StringS.h"

int main() {
    using namespace std;

    string input;
    cout << "Введіть рядок: ";
    getline(cin, input);

    StringManipulator sm(input);

    cout << "Початковий рядок: " << sm.getText() << endl;
    cout << "Довжина рядка: " << sm.calculateLength() << endl;

    sm.shiftRight();
    cout << "Рядок після зсуву праворуч: " << sm.getText() << endl;

    return 0;
}