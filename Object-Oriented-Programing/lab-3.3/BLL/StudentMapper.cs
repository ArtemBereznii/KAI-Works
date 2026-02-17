using BLL.DTO;
using DAL.Entities;
using System.Collections.Generic;
using System.Linq;

namespace BLL.Infrastructure
{
    // Допоміжний клас для перетворення DAL.Student <-> BLL.StudentDTO
    public static class StudentMapper
    {
        public static StudentDTO ToDto(this Student student)
        {
            return new StudentDTO
            {
                LastName = student.LastName,
                FirstName = student.FirstName,
                Course = student.Course,
                TicketID = student.TicketID,
                BirthDate = student.BirthDate
            };
        }

        public static Student ToEntity(this StudentDTO dto)
        {
            return new Student
            {
                LastName = dto.LastName,
                FirstName = dto.FirstName,
                Course = dto.Course,
                TicketID = dto.TicketID,
                BirthDate = dto.BirthDate
            };
        }

        public static IEnumerable<StudentDTO> ToDto(this IEnumerable<Student> students)
        {
            return students.Select(s => s.ToDto());
        }

        public static IEnumerable<Student> ToEntity(this IEnumerable<StudentDTO> dtos)
        {
            return dtos.Select(dto => dto.ToEntity());
        }
    }
}