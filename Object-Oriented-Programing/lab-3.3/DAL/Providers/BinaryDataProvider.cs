using DAL.Abstractions;
using DAL.Entities;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace DAL.Providers
{
    public class BinaryDataProvider : IDataProvider<Student>
    {
        public IEnumerable<Student> Load(string filePath)
        {
            if (!File.Exists(filePath))
                return new List<Student>();

            // BinaryFormatter застарів, але часто вимагається в лабах
            #pragma warning disable SYSLIB0011
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                return (IEnumerable<Student>)formatter.Deserialize(fs);
            }
            #pragma warning restore SYSLIB0011
        }

        public void Save(string filePath, IEnumerable<Student> data)
        {
            #pragma warning disable SYSLIB0011
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                formatter.Serialize(fs, data);
            }
            #pragma warning restore SYSLIB0011
        }
    }
}