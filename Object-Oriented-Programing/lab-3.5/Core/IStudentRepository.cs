using Core.Entities;
using System.Collections.Generic;

namespace Core.Interfaces
{
    // Контракт для DAL: що він *повинен* вміти робити з даними
    public interface IStudentRepository
    {
        IEnumerable<Student> GetStudents();
        void SaveStudents(IEnumerable<Student> students);
    }
}