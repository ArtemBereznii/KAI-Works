using System;
using System.Linq;

namespace TextLibrary
{
    public class TextLine : IDigits
    {
        private readonly string content;

        public TextLine(string line)
        {
            content = line;
        }

        public string Content => content;

        public string GetDigitsString()
        {
            return new string(content.Where(char.IsDigit).ToArray());
        }
    }
}