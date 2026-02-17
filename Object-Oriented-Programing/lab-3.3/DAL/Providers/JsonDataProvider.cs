using DAL.Abstractions;
using DAL.Entities;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace DAL.Providers
{
    public class JsonDataProvider : IDataProvider<Student>
    {
        public IEnumerable<Student> Load(string filePath)
        {
            if (!File.Exists(filePath))
                return new List<Student>();

            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Student>>(json) ?? new List<Student>();
        }

        public void Save(string filePath, IEnumerable<Student> data)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(data, options);
            File.WriteAllText(filePath, json);
        }
    }
}