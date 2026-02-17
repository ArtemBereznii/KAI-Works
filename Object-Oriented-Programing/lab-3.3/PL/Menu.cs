using BLL.DTO;
using BLL.Exceptions;
using BLL.Services;
using DAL.Abstractions;
using DAL.Entities;
using DAL.Providers;
using System;
using System.Collections.Generic;
using System.IO;

namespace PL
{
    public class Menu
    {
        private readonly StudentService _studentService;

        public Menu(StudentService studentService)
        {
            _studentService = studentService;
        }

        public void MainMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("--- ГОЛОВНЕ МЕНЮ ---");
                Console.WriteLine("1. Додати нового студента");
                Console.WriteLine("2. Показати всіх студентів");
                Console.WriteLine("3. Виконати завдання (студенти 3 курсу, народжені влітку)");
                Console.WriteLine("4. Зберегти дані у файл");
                Console.WriteLine("5. Завантажити дані з файлу");
                Console.WriteLine("0. Вихід");

                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.D1: CreateStudent(); break;
                    case ConsoleKey.D2: ShowAllStudents(); break;
                    case ConsoleKey.D3: RunTask(); break;
                    case ConsoleKey.D4: SaveData(); break;
                    case ConsoleKey.D5: LoadData(); break;
                    case ConsoleKey.D0: return;
                }
            }
        }

        private void CreateStudent()
        {
            try
            {
                Console.WriteLine("\n--- Додавання студента ---");
                var dto = new StudentDTO();
                dto.LastName = ReadString("Прізвище:");
                dto.FirstName = ReadString("Ім'я:");
                dto.Course = ReadInt("Курс:");
                dto.TicketID = ReadString("Номер студ. квитка:");
                dto.BirthDate = ReadDate("Дата народження (дд.мм.рррр):");

                _studentService.AddNewStudent(dto);
                Console.WriteLine("\nСтудента успішно додано. Натисніть Enter.");
            }
            // Обробка власного виключення з BLL 
            catch (DataValidationException ex)
            {
                Console.WriteLine($"\n[Помилка валідації] {ex.Message} (Поле: {ex.FieldName})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[Критична помилка] {ex.Message}");
            }
            Console.ReadKey(true);
        }

        private void ShowAllStudents()
        {
            Console.WriteLine("\n--- Список студентів ---");
            var students = _studentService.GetAllStudents();
            int i = 0;
            foreach (var s in students)
            {
                Console.WriteLine($"{++i}. {s.FullName}, {s.Course} курс, Квиток: {s.TicketID}, ДН: {s.BirthDate:d}");
            }
            if (i == 0) Console.WriteLine("Список порожній.");
            Console.WriteLine("\nНатисніть Enter.");
            Console.ReadKey(true);
        }

        private void RunTask()
        {
            Console.WriteLine("\n--- Завдання: Студенти 3-го курсу, народжені влітку ---");
            try
            {
                var students = _studentService.GetSummerBornThirdYears();
                int i = 0;
                foreach (var s in students)
                {
                    Console.WriteLine($"{++i}. {s.FullName}, {s.Course} курс, ДН: {s.BirthDate:d}");
                }
                Console.WriteLine($"\n[Всього знайдено: {i}]");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"[Помилка] {ex.Message}");
            }
            Console.WriteLine("\nНатисніть Enter.");
            Console.ReadKey(true);
        }

        private void SaveData()
        {
            try
            {
                var (filePath, provider) = SelectProvider();
                _studentService.SaveStudents(filePath, provider);
                Console.WriteLine($"\nДані успішно збережено у файл {filePath}. Натисніть Enter.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[Помилка збереження] {ex.Message}");
            }
            Console.ReadKey(true);
        }

        private void LoadData()
        {
            try
            {
                var (filePath, provider) = SelectProvider();
                if (!File.Exists(filePath))
                    throw new FileNotFoundException("Файл не знайдено!", filePath);

                _studentService.LoadStudents(filePath, provider);
                Console.WriteLine($"\nДані успішно завантажено з файлу {filePath}. Натисніть Enter.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[Помилка завантаження] {ex.Message}");
            }
            Console.ReadKey(true);
        }

        // --- Допоміжні методи PL ---

        // PL відповідає за вибір файлу та провайдера і передає їх на нижчі рівні 
        private (string, IDataProvider<Student>) SelectProvider()
        {
            Console.WriteLine("\n--- Вибір типу файлу ---");
            Console.WriteLine("1. Binary (.bin)");
            Console.WriteLine("2. XML (.xml)");
            Console.WriteLine("3. JSON (.json)");
            Console.WriteLine("4. Custom CSV (.csv)");
            var key = Console.ReadKey(true).Key;

            Console.Write("Введіть ім'я файлу (без розширення): ");
            string fileName = Console.ReadLine() ?? string.Empty;

            switch (key)
            {
                case ConsoleKey.D1: return (fileName + ".bin", new BinaryDataProvider());
                case ConsoleKey.D2: return (fileName + ".xml", new XmlDataProvider());
                case ConsoleKey.D3: return (fileName + ".json", new JsonDataProvider());
                case ConsoleKey.D4: return (fileName + ".csv", new CustomCsvDataProvider());
                default: throw new InvalidOperationException("Невірний вибір.");
            }
        }

        private string ReadString(string prompt)
        {
            Console.Write(prompt + " ");
            return Console.ReadLine() ?? string.Empty;
        }

        private int ReadInt(string prompt)
        {
            while (true)
            {
                try { return int.Parse(ReadString(prompt)); }
                catch { Console.WriteLine("Невірний формат, введіть число."); }
            }
        }

        private DateTime ReadDate(string prompt)
        {
            while (true)
            {
                try { return DateTime.Parse(ReadString(prompt)); }
                catch { Console.WriteLine("Невірний формат, введіть дату (дд.мм.рррр)."); }
            }
        }
    }
}