using System;

namespace Restaurant.BLL.Exceptions
{
    public class EntityInUseException : Exception
    {
        public EntityInUseException(string message) : base(message) { }
    }
}