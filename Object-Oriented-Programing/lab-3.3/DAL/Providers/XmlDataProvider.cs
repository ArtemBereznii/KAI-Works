using DAL.Abstractions;
using DAL.Entities;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace DAL.Providers
{
    public class XmlDataProvider : IDataProvider<Student>
    {
        public IEnumerable<Student> Load(string filePath)
        {
            if (!File.Exists(filePath))
                return new List<Student>();

            XmlSerializer serializer = new XmlSerializer(typeof(List<Student>));
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                return (IEnumerable<Student>?)serializer.Deserialize(fs) ?? new List<Student>();
            }
        }

        public void Save(string filePath, IEnumerable<Student> data)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Student>));
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                serializer.Serialize(fs, data);
            }
        }
    }
}