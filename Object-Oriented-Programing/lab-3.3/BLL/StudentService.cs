using BLL.DTO;
using BLL.Exceptions;
using BLL.Infrastructure;
using DAL;
using DAL.Abstractions;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL.Services
{
    public class StudentService
    {
        private readonly EntityContext _context;
        private List<StudentDTO> _students; // Внутрішній кеш даних

        public StudentService(EntityContext context)
        {
            _context = context;
            _students = new List<StudentDTO>();
        }

        public void AddNewStudent(StudentDTO studentDto)
        {
            // Валідація даних (частина бізнес-логіки)
            if (string.IsNullOrWhiteSpace(studentDto.LastName))
                throw new DataValidationException("Прізвище не може бути порожнім", nameof(studentDto.LastName));
            if (studentDto.Course < 1 || studentDto.Course > 5)
                throw new DataValidationException("Курс має бути від 1 до 5", nameof(studentDto.Course));

            _students.Add(studentDto);
        }

        public IEnumerable<StudentDTO> GetAllStudents()
        {
            return _students;
        }

        // --- ЛОГІКА ЗАВДАННЯ ВАРІАНТУ ---
        public IEnumerable<StudentDTO> GetSummerBornThirdYears()
        {
            if (!_students.Any())
                throw new InvalidOperationException("Список студентів порожній. Завантажте дані.");

            return _students.Where(s =>
                s.Course == 3 &&
                (s.BirthDate.Month == 6 || s.BirthDate.Month == 7 || s.BirthDate.Month == 8)
            );
        }

        // --- Робота з DAL ---
        public void SaveStudents(string filePath, IDataProvider<Student> provider)
        {
            // Сервіс працює з DTO, але в DAL передає Entities
            var entities = _students.ToEntity();
            _context.SaveStudents(filePath, entities, provider);
        }

        public void LoadStudents(string filePath, IDataProvider<Student> provider)
        {
            // Сервіс отримує Entities з DAL, але в кеші зберігає DTO
            var entities = _context.LoadStudents(filePath, provider);
            _students = entities.ToDto().ToList();
        }
    }
}