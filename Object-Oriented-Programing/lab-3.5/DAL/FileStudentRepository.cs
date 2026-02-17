using Core.Entities;
using Core.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace DAL.Repositories
{
    // Справжня реалізація DAL, яка працює з файлами
    public class FileStudentRepository : IStudentRepository
    {
        private readonly string _filePath;

        public FileStudentRepository(string filePath)
        {
            _filePath = filePath;
        }

        public IEnumerable<Student> GetStudents()
        {
            if (!File.Exists(_filePath))
                return new List<Student>();

            string json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<Student>>(json) ?? new List<Student>();
        }

        public void SaveStudents(IEnumerable<Student> students)
        {
            string json = JsonSerializer.Serialize(students, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
    }
}