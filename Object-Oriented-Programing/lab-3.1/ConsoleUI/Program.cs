using System;
using System.IO;
using FileLibrary;

namespace ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            string dataPath = @"C:\Users\Artem\source\repos\oop\lab-3.1\data.txt";

            IFileHandler fileHandler = new FileHandler(dataPath);
            var menu = new ConsoleMenu(fileHandler);
            menu.Run();
        }
    }
}
