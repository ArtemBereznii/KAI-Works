using DAL.Abstractions;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DAL.Providers
{
    // Реалізуємо "користувацьку" серіалізацію як простий CSV-файл
    public class CustomCsvDataProvider : IDataProvider<Student>
    {
        public IEnumerable<Student> Load(string filePath)
        {
            if (!File.Exists(filePath))
                yield break; // Повертаємо порожню послідовність

            var lines = File.ReadAllLines(filePath).Skip(1); // Пропускаємо заголовок
            foreach (var line in lines)
            {
                var parts = line.Split(',');
                if (parts.Length == 5)
                {
                    yield return new Student
                    {
                        LastName = parts[0],
                        FirstName = parts[1],
                        Course = int.Parse(parts[2]),
                        TicketID = parts[3],
                        BirthDate = DateTime.Parse(parts[4])
                    };
                }
            }
        }

        public void Save(string filePath, IEnumerable<Student> data)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("LastName,FirstName,Course,TicketID,BirthDate"); // Заголовок
            foreach (var student in data)
            {
                sb.AppendLine($"{student.LastName},{student.FirstName},{student.Course},{student.TicketID},{student.BirthDate:O}");
            }
            File.WriteAllText(filePath, sb.ToString());
        }
    }
}