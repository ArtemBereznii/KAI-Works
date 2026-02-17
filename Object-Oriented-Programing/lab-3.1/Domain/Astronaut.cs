using Domain.Interfaces;

namespace Domain
{
    public class Astronaut : Person, ISinger
    {
        public Astronaut(string firstName, string lastName) : base(firstName, lastName) { }

        public void FlyToSpace() { }

        public void Sing() { }
    }
}
