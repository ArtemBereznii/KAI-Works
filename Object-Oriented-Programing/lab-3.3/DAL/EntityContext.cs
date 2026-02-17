using DAL.Abstractions;
using DAL.Entities;
using System.Collections.Generic;

namespace DAL
{
    // Цей клас відповідає за фізичну роботу з даними 
    public class EntityContext
    {
        // Метод, що приймає будь-який провайдер
        public void SaveStudents(string filePath, IEnumerable<Student> students, IDataProvider<Student> provider)
        {
            provider.Save(filePath, students);
        }

        public IEnumerable<Student> LoadStudents(string filePath, IDataProvider<Student> provider)
        {
            return provider.Load(filePath);
        }
    }
}