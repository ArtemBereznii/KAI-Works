using System;
using System.Text.RegularExpressions;
using System.Globalization;
using Domain;
using FileLibrary;

namespace ConsoleUI
{
    public class ConsoleMenu
    {
        private readonly IFileHandler _fileHandler;

        public ConsoleMenu(IFileHandler fileHandler)
        {
            _fileHandler = fileHandler;
        }

        public void Run()
        {
            while (true)
            {
                Console.WriteLine("\n=== Меню ===");
                Console.WriteLine("1. Додати студента");
                Console.WriteLine("2. Показати всіх сутностей");
                Console.WriteLine("3. Знайти студентів 3-го курсу, народжених влітку");
                Console.WriteLine("0. Вийти");
                Console.Write("Ваш вибір: ");
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": AddStudent(); break;
                    case "2": ShowAll(); break;
                    case "3": ShowSummer3rdCourse(); break;
                    case "0": return;
                }
            }
        }

        private void AddStudent()
        {
            Console.Write("Ім'я: ");
            string? first = Console.ReadLine();
            Console.Write("Прізвище: ");
            string? last = Console.ReadLine();
            Console.Write("Курс: ");
            int course = int.Parse(Console.ReadLine() ?? "1");
            Console.Write("StudentID (KB123456): ");
            string? id = Console.ReadLine();
            Console.Write("Дата народження (dd-MM-yyyy): ");
            DateTime birth = DateTime.ParseExact(Console.ReadLine() ?? "01-01-2000", "dd-MM-yyyy", CultureInfo.InvariantCulture);

            if (!Regex.IsMatch(id ?? "", @"^[A-Z]{2}\d{6}$", RegexOptions.IgnoreCase))
            {
                Console.WriteLine("Невірний формат StudentID");
                return;
            }

            var student = new Student(first ?? "", last ?? "", course, id ?? "XX000000", birth);
            _fileHandler.SavePerson(student);
            Console.WriteLine("Студента збережено.");
        }

        private void ShowAll()
        {
            var persons = _fileHandler.LoadAllPersons();
            Console.WriteLine("\n=== Всі сутності ===");
            foreach (var p in persons)
            {
                if (p is Student s)
                    Console.WriteLine($"Student: {s.FirstName} {s.LastName}, {s.Course} курс, {s.StudentId}, {s.BirthDate:dd-MM-yyyy}");
                else if (p is Teacher t)
                    Console.WriteLine($"Teacher: {t.FirstName} {t.LastName}");
                else if (p is Astronaut a)
                    Console.WriteLine($"Astronaut: {a.FirstName} {a.LastName}");
            }
        }

        private void ShowSummer3rdCourse()
        {
            var persons = _fileHandler.LoadAllPersons();
            Console.WriteLine("\n=== Студенти 3-го курсу, народжені влітку ===");
            int count = 0;
            foreach (var p in persons)
            {
                if (p is Student s && s.Course == 3 && s.IsSummerBorn())
                {
                    Console.WriteLine($"{s.FirstName} {s.LastName}, {s.StudentId}, {s.BirthDate:dd-MM-yyyy}");
                    count++;
                }
            }
            Console.WriteLine($"Всього: {count}");
        }
    }
}
