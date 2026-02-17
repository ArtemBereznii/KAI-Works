#include <iostream>
#include "Segment.h"

int main() {
    Segment segment1; 
    Segment segment2(0, 0, 3, 4);
    Segment segment3(segment2);

    std::cout << "Segment 1 (default): (" << segment1.getX1() << ", " << segment1.getY1()
        << ") to (" << segment1.getX2() << ", " << segment1.getY2() << ")\n";

    std::cout << "Segment 2: Length = " << segment2.calculateLength()
        << ", Angle with OY = " << segment2.calculateAngleWithOY() << " degrees\n";

    std::cout << "Segment 3 (copy): Length = " << segment3.calculateLength() << "\n";

    return 0;
}