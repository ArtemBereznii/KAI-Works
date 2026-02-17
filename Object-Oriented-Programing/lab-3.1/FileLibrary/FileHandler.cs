using System;
using System.IO;
using System.Text.RegularExpressions;
using Domain;

namespace FileLibrary
{
    public class FileHandler : IFileHandler
    {
        private readonly string _filePath;

        public FileHandler(string filePath)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }

        public void SavePerson(Person person)
        {
            using (var fs = new FileStream(_filePath, FileMode.Append, FileAccess.Write, FileShare.None))
            using (var sw = new StreamWriter(fs))
            {
                sw.WriteLine($"{person.GetType().Name} {person.FirstName}{person.LastName}");

                if (person is Student s)
                {
                    sw.WriteLine(
                        $"{{ \"firstname\": \"{s.FirstName}\", \"lastname\": \"{s.LastName}\", \"course\": \"{s.Course}\", \"studentId\": \"{s.StudentId}\", \"birthdate\": \"{s.BirthDate:dd-MM-yyyy}\" }};"
                    );
                }
                else if (person is Teacher t)
                {
                    sw.WriteLine($"{{ \"firstname\": \"{t.FirstName}\", \"lastname\": \"{t.LastName}\" }};");
                }
                else if (person is Astronaut a)
                {
                    sw.WriteLine($"{{ \"firstname\": \"{a.FirstName}\", \"lastname\": \"{a.LastName}\" }};");
                }
            }
        }

        public Person[] LoadAllPersons()
        {
            if (!File.Exists(_filePath))
                return Array.Empty<Person>();

            var rawLines = File.ReadAllLines(_filePath);
            int records = rawLines.Length / 2;
            Person[] result = new Person[records];
            int idx = 0;

            for (int i = 0; i + 1 < rawLines.Length; i += 2)
            {
                string header = rawLines[i].Trim();
                if (string.IsNullOrWhiteSpace(header)) continue;

                string data = rawLines[i + 1].Trim();
                var headerParts = header.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                if (headerParts.Length == 0) continue;

                string type = headerParts[0];

                if (type == "Student")
                {
                    var m = Regex.Match(data,
                        @"\""firstname\"":\s*\""(?<first>[^\""]+)\"".*\""lastname\"":\s*\""(?<last>[^\""]+)\"".*\""course\"":\s*\""(?<course>\d+)\"".*\""studentId\"":\s*\""(?<id>[^\""]+)\"".*\""birthdate\"":\s*\""(?<birth>[^\""]+)\"""
                    );

                    if (m.Success)
                    {
                        result[idx++] = new Student(
                            m.Groups["first"].Value,
                            m.Groups["last"].Value,
                            int.Parse(m.Groups["course"].Value),
                            m.Groups["id"].Value,
                            DateTime.ParseExact(m.Groups["birth"].Value, "dd-MM-yyyy", null)
                        );
                    }
                }
                else if (type == "Teacher")
                {
                    var m = Regex.Match(data,
                        @"\""firstname\"":\s*\""(?<first>[^\""]+)\"".*\""lastname\"":\s*\""(?<last>[^\""]+)\"""
                    );

                    if (m.Success)
                    {
                        result[idx++] = new Teacher(m.Groups["first"].Value, m.Groups["last"].Value);
                    }
                }
                else if (type == "Astronaut")
                {
                    var m = Regex.Match(data,
                        @"\""firstname\"":\s*\""(?<first>[^\""]+)\"".*\""lastname\"":\s*\""(?<last>[^\""]+)\"""
                    );

                    if (m.Success)
                    {
                        result[idx++] = new Astronaut(m.Groups["first"].Value, m.Groups["last"].Value);
                    }
                }
            }

            if (idx != result.Length)
                Array.Resize(ref result, idx);

            return result;
        }
    }
}
