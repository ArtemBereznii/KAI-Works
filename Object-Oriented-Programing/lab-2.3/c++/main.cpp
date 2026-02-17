#include <iostream>
#include "TextContainer.h"
#include "TextLine.h"

void displayMenu() {
    std::cout << "\nText Container Operations:\n";
    std::cout << "1. Add line\n";
    std::cout << "2. Remove line\n";
    std::cout << "3. Replace line\n";
    std::cout << "4. Clear text\n";
    std::cout << "5. Show line count\n";
    std::cout << "6. Extract all digits\n";
    std::cout << "7. Exit\n";
    std::cout << "Choose option: ";
}

int main() {
    TextContainer container;
    int choice;
    std::string input;

    do {
        displayMenu();
        std::cin >> choice;
        std::cin.ignore();

        switch (choice) {
        case 1: {
            std::cout << "Enter line to add: ";
            std::getline(std::cin, input);
            container.addLine(TextLine(input));
            std::cout << "Line added successfully.\n";
            break;
        }
        case 2: {
            size_t index;
            std::cout << "Enter line index to remove: ";
            std::cin >> index;
            container.removeLine(index);
            std::cout << "Line removed (if index was valid).\n";
            break;
        }
        case 3: {
            size_t index;
            std::cout << "Enter line index to replace: ";
            std::cin >> index;
            std::cin.ignore();
            std::cout << "Enter new line content: ";
            std::getline(std::cin, input);
            container.replaceLine(index, TextLine(input));
            std::cout << "Line replaced (if index was valid).\n";
            break;
        }
        case 4:
            container.clearText();
            std::cout << "Text cleared.\n";
            break;
        case 5:
            std::cout << "Total lines: " << container.getLinesCount() << "\n";
            break;
        case 6:
            std::cout << "All digits in text: " << container.getAllDigits() << "\n";
            break;
        case 7:
            std::cout << "Exiting program.\n";
            break;
        default:
            std::cout << "Invalid option. Try again.\n";
        }
    } while (choice != 7);

    return 0;
}