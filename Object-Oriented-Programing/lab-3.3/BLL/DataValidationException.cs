using System;

namespace BLL.Exceptions
{
    // Власний клас виключення 
    public class DataValidationException : Exception
    {
        public string FieldName { get; }

        public DataValidationException(string message, string fieldName) : base(message)
        {
            FieldName = fieldName;
        }
    }
}