using System;

public class StringManipulator
{
    private string text;

    public StringManipulator()
    {
        text = "";
    }

    public StringManipulator(string str)
    {
        text = str;
    }

    public StringManipulator(StringManipulator other)
    {
        text = other.text;
    }

    public int CalculateLength()
    {
        return text.Length;
    }

    public void ShiftRight()
    {
        if (!string.IsNullOrEmpty(text))
        {
            char lastChar = text[text.Length - 1];
            text = lastChar + text.Substring(0, text.Length - 1);
        }
    }

    public string GetText()
    {
        return text;
    }
}