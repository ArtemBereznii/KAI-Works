using System;

namespace LabOOP3
{
    public class Student : IComparable<Student>
    {
        public string TicketNumber { get; set; }
        public string FullName { get; set; }
        public int BirthYear { get; set; }
        public string GroupNumber { get; set; }
        public int Course { get; private set; }

        public Student(string ticketNumber, string fullName, int birthYear, string groupNumber, int initialCourse = 1)
        {
            TicketNumber = ticketNumber;
            FullName = fullName;
            BirthYear = birthYear;
            GroupNumber = groupNumber;
            Course = initialCourse;
        }

        public void AdvanceCourse()
        {
            if (Course < 5) 
            {
                Course++;
                Console.WriteLine($">>> Студента {FullName} переведено на {Course} курс.");
            }
            else
            {
                Console.WriteLine($">>> Студент {FullName} вже є випускником.");
            }
        }

        public int CalculateAge()
        {
            return DateTime.Now.Year - BirthYear;
        }

        public void DisplayInfo()
        {
            Console.WriteLine("------------------------------");
            Console.WriteLine($"Студент: {FullName}");
            Console.WriteLine($"  Номер квитка: {TicketNumber}");
            Console.WriteLine($"  Рік народження: {BirthYear} (Вік: {CalculateAge()})");
            Console.WriteLine($"  Група: {GroupNumber}");
            Console.WriteLine($"  Курс: {Course}");
            Console.WriteLine("------------------------------");
        }

        public int CompareTo(Student? other)
        {
            if (other == null) return 1;
            // Використовуємо вбудований метод порівняння рядків
            return string.Compare(this.TicketNumber, other.TicketNumber, StringComparison.Ordinal);
        }
    }
}