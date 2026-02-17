using Restaurant.BLL.Models;
using System.Collections.Generic;

namespace Restaurant.BLL.Interfaces
{
    public interface IRepository<T> where T : class, IEntity
    {
        T? GetById(int id);
        IEnumerable<T> GetAll();
        void Add(T entity);
        void Update(T entity);
        void Delete(int id);
    }
}