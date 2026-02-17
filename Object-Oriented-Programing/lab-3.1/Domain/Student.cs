using System;
using Domain.Interfaces;

namespace Domain
{
    public class Student : Person, IStudent, ISinger
    {
        public int Course { get; }
        public string StudentId { get; }
        public DateTime BirthDate { get; }

        public Student(string firstName, string lastName, int course, string studentId, DateTime birthDate)
            : base(firstName, lastName)
        {
            if (course <= 0) throw new ArgumentOutOfRangeException(nameof(course));
            if (string.IsNullOrWhiteSpace(studentId)) throw new ArgumentException("StudentId required", nameof(studentId));

            Course = course;
            StudentId = studentId;
            BirthDate = birthDate;
        }

        public void Study() { }

        public void Sing() { }

        public bool IsSummerBorn() =>
            BirthDate.Month == 6 || BirthDate.Month == 7 || BirthDate.Month == 8;
    }
}
