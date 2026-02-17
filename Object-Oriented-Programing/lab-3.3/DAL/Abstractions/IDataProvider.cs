using System.Collections.Generic;

namespace DAL.Abstractions
{
    // Узагальнений інтерфейс для роботи з будь-яким типом даних
    public interface IDataProvider<T> where T : class
    {
        void Save(string filePath, IEnumerable<T> data);
        IEnumerable<T> Load(string filePath);
    }
}