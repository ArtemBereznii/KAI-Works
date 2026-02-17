using System;

namespace Domain
{
    public abstract class Person
    {
        public string FirstName { get; }
        public string LastName { get; }

        protected Person(string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName)) throw new ArgumentException("FirstName required", nameof(firstName));
            if (string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("LastName required", nameof(lastName));

            FirstName = firstName;
            LastName = lastName;
        }
    }
}
