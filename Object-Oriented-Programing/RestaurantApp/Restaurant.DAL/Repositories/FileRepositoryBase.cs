using Restaurant.BLL.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Restaurant.DAL.Repositories
{
    public abstract class FileRepositoryBase<T> where T : class, IEntity
    {
        protected readonly string _filePath;
        protected List<T> _entities;

        protected FileRepositoryBase(string filePath)
        {
            _filePath = filePath;
            _entities = new List<T>();
            LoadData();
        }

        // Завантажуємо дані з файлу
        protected void LoadData()
        {
            if (!File.Exists(_filePath))
            {
                _entities = new List<T>();
                return;
            }

            try
            {
                string json = File.ReadAllText(_filePath);
                _entities = JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка читання файлу {_filePath}: {ex.Message}");
                _entities = new List<T>();
            }
        }

        // Зберігаємо дані у файл
        protected void SaveData()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(_entities, options);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка запису у файл {_filePath}: {ex.Message}");
            }
        }

        public T? GetById(int id)
        {
            return _entities.FirstOrDefault(e => e.Id == id);
        }

        public IEnumerable<T> GetAll()
        {
            return _entities.AsReadOnly();
        }

        public void Add(T entity)
        {
            // Генеруємо новий Id
            entity.Id = _entities.Any() ? _entities.Max(e => e.Id) + 1 : 1;
            _entities.Add(entity);
            SaveData();
        }

        public void Update(T entity)
        {
            var existing = _entities.FirstOrDefault(e => e.Id == entity.Id);
            if (existing != null)
            {
                var index = _entities.IndexOf(existing);
                _entities[index] = entity;
                SaveData();
            }
        }

        public void Delete(int id)
        {
            var entity = _entities.FirstOrDefault(e => e.Id == id);
            if (entity != null)
            {
                _entities.Remove(entity);
                SaveData();
            }
        }
    }
}