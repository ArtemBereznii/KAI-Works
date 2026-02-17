using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace DAL.Entities
{
    // [Serializable] потрібен для BinaryFormatter
    [Serializable]
    public class Student
    {
        // Властивості для XML/JSON серіалізації
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string TicketID { get; set; } = string.Empty;
        public int Course { get; set; }
        public DateTime BirthDate { get; set; }

        // Потрібен для XML та JSON серіалізаторів
        public Student() { }

        // Конструктор для логіки
        public Student(string lastName, string firstName, int course, string ticketID, DateTime birthDate)
        {
            LastName = lastName;
            FirstName = firstName;
            Course = course;
            TicketID = ticketID;
            BirthDate = birthDate;
        }
    }
}