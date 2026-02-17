using Domain.Interfaces;

namespace Domain
{
    public class Teacher : Person, ITeacherActions, ISinger
    {
        public Teacher(string firstName, string lastName) : base(firstName, lastName) { }

        public void Teach() { }

        public void Sing() { }
    }
}
