using BLL;
using Core.Entities;
using Core.Interfaces;
using DAL.Repositories;
using DAL.Services;
using System;

namespace PL_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // --- Налаштування Dependency Injection (вручну) ---
            // 1. Створюємо реальні сервіси DAL
            IStudentRepository repo = new FileStudentRepository("students.json");
            IInternetService net = new ExternalInternetService();

            // 2. "Ін'єктуємо" (передаємо) їх у BLL
            StudentService studentService = new StudentService(repo, net);

            // --- Демонстрація ---
            Console.WriteLine("--- Демонстрація роботи сервісу ---");

            var student = new Student
            {
                FirstName = "Петро",
                Course = 5
            };

            Console.WriteLine($"{student.FirstName}, Курс: {student.Course}");
            studentService.AdvanceStudentCourse(student);
            Console.WriteLine($"Після переведення: {student.Course}");

            studentService.AdvanceStudentCourse(student);
            Console.WriteLine($"Спроба перевести з 6-го курсу: {student.Course}");

            Console.WriteLine(studentService.Sing(student));
            Console.WriteLine(studentService.StudyOnline(student));
        }
    }
}