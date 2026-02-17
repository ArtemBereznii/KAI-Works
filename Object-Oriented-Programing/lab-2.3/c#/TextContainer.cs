using System.Collections.Generic;
using System.Linq;

namespace TextLibrary
{
    public class TextContainer
    {
        private readonly List<TextLine> lines = new List<TextLine>();

        public void AddLine(TextLine line)
        {
            lines.Add(line);
        }

        public void RemoveLine(int index)
        {
            if (index >= 0 && index < lines.Count)
            {
                lines.RemoveAt(index);
            }
        }

        public void ReplaceLine(int index, TextLine newLine)
        {
            if (index >= 0 && index < lines.Count)
            {
                lines[index] = newLine;
            }
        }

        public void ClearText()
        {
            lines.Clear();
        }

        public int GetLinesCount()
        {
            return lines.Count;
        }

        public string GetAllDigits()
        {
            return string.Join("", lines.Select(line => line.GetDigitsString()));
        }
    }
}