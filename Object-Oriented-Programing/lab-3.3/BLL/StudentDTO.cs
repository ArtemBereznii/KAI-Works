using System;

namespace BLL.DTO
{
    // "Чистий" об'єкт, який BLL та PL використовують для обміну даними.
    // Він нічого не знає про серіалізацію чи базу даних.
    public class StudentDTO
    {
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string TicketID { get; set; } = string.Empty;
        public int Course { get; set; }
        public DateTime BirthDate { get; set; }

        public string FullName => $"{LastName} {FirstName}";
    }
}