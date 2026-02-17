using System;
using System.Linq;

namespace StringsLibrary
{
    public class Strings
    {
        protected string value;

        public Strings(string val)
        {
            value = val;
        }

        public string Value => value;

        public virtual int CalculateLength()
        {
            return value.Length;
        }

        public virtual string SortAndReturn()
        {
            char[] chars = value.ToCharArray();
            Array.Sort(chars);
            return new string(chars);
        }
    }

    public class UppercaseLetters : Strings
    {
        public UppercaseLetters(string val) : base(val) { }

        public override int CalculateLength()
        {
            return value.Length;
        }

        public override string SortAndReturn()
        {
            char[] chars = value.ToCharArray();
            Array.Sort(chars);
            return new string(chars);
        }
    }

    public class LowercaseLetters : Strings
    {
        public LowercaseLetters(string val) : base(val) { }

        public override int CalculateLength()
        {
            return value.Length;
        }

        public override string SortAndReturn()
        {
            char[] chars = value.ToCharArray();
            Array.Sort(chars);
            Array.Reverse(chars);
            return new string(chars);
        }
    }
}