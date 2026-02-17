using Restaurant.BLL.Interfaces;
using Restaurant.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Restaurant.Tests.Additional
{
    // Фейковий репозиторій для інгредієнтів
    public class FakeIngredientRepository : IIngredientRepository
    {
        public List<Ingredient> Data = new List<Ingredient>();

        public void Add(Ingredient entity)
        {
            entity.Id = Data.Any() ? Data.Max(i => i.Id) + 1 : 1;
            Data.Add(entity);
        }

        public void Delete(int id) => Data.RemoveAll(i => i.Id == id);

        public IEnumerable<Ingredient> GetAll() => Data;

        public Ingredient? GetById(int id) => Data.FirstOrDefault(i => i.Id == id);

        public IEnumerable<Ingredient> Search(string keyword) =>
            Data.Where(i => i.Name != null && i.Name.Contains(keyword));

        public void Update(Ingredient entity)
        {
            var existing = GetById(entity.Id);
            if (existing != null)
            {
                Data.Remove(existing);
                Data.Add(entity);
            }
        }
    }

    // Фейковий репозиторій для страв
    public class FakeDishRepository : IDishRepository
    {
        public List<Dish> Data = new List<Dish>();

        public void Add(Dish entity)
        {
            entity.Id = Data.Any() ? Data.Max(d => d.Id) + 1 : 1;
            Data.Add(entity);
        }

        public void Delete(int id) => Data.RemoveAll(d => d.Id == id);

        public IEnumerable<Dish> GetAll() => Data;

        public Dish? GetById(int id) => Data.FirstOrDefault(d => d.Id == id);

        public void Update(Dish entity)
        {
            var existing = GetById(entity.Id);
            if (existing != null)
            {
                Data.Remove(existing);
                Data.Add(entity);
            }
        }
    }

    // Фейковий репозиторій для замовлень
    public class FakeOrderRepository : IOrderRepository
    {
        public List<Order> Data = new List<Order>();

        public void Add(Order entity)
        {
            entity.Id = Data.Any() ? Data.Max(o => o.Id) + 1 : 1;
            Data.Add(entity);
        }

        public void Delete(int id) => Data.RemoveAll(o => o.Id == id);

        public IEnumerable<Order> GetAll() => Data;

        public Order? GetById(int id) => Data.FirstOrDefault(o => o.Id == id);

        public IEnumerable<Order> Search(string keyword) => new List<Order>(); // Спрощено для тестів

        public void Update(Order entity)
        {
            var existing = GetById(entity.Id);
            if (existing != null)
            {
                Data.Remove(existing);
                Data.Add(entity);
            }
        }
    }
}