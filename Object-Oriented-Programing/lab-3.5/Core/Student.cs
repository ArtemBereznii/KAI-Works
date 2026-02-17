using System;

namespace Core.Entities
{
    // Сутність студента.
    public class Student
    {
        public string TicketID { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int Course { get; set; }
        public DateTime BirthDate { get; set; }
    }
}