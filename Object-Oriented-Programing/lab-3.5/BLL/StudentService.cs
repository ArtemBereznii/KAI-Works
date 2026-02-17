using Core.Entities;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    public class StudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IInternetService _internetService;

        // Використовуємо Dependency Injection: отримуємо залежності через конструктор
        public StudentService(IStudentRepository studentRepository, IInternetService internetService)
        {
            _studentRepository = studentRepository;
            _internetService = internetService;
        }

        // --- Основна логіка ---
        public IEnumerable<Student> GetSummerBornThirdYears()
        {
            var students = _studentRepository.GetStudents();
            return students.Where(s =>
                s.Course == 3 &&
                (s.BirthDate.Month >= 6 && s.BirthDate.Month <= 8)
            );
        }

        public void AdvanceStudentCourse(Student student)
        {
            if (student.Course < 6)
            {
                student.Course++;
            }
        }

        // --- Додаткові вміння ---
        public string Sing(Student student)
        {
            return $"{student.FirstName} співає: Ла-ла-ла!";
        }

        // --- Додаткові сутності ---
        public string StudyOnline(Student student)
        {
            if (_internetService.IsConnected())
            {
                return $"{student.FirstName} вчиться онлайн.";
            }
            return $"{student.FirstName} не може вчитись: немає інтернету.";
        }

        public string StudyOnline(Teacher teacher)
        {
            if (_internetService.IsConnected())
            {
                return $"{teacher.Name} (вчитель) вчиться онлайн.";
            }
            return $"{teacher.Name} (вчитель) не може вчитись: немає інтернету.";
        }
    }
}