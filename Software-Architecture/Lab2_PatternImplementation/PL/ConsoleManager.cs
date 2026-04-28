using System;
using HardwareSim.BLL.Features.Events;

namespace HardwareSim.PL
{
    public static class ConsoleManager
    {
        public static void PrintMenu(string[] options, string title = "MENU")
        {
            Console.WriteLine();
            Console.WriteLine($"--- {title} ---");
            for (int i = 0; i < options.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {options[i]}");
            }
            Console.Write("Select an option: ");
        }

        public static void PrintInfo(string message)
        {
            Console.WriteLine($"[INFO] {message}");
        }

        // The Observer
        public static void HandleDeviceNotification(object? sender, HardwareEventArgs? e)
        {
            if (e == null) return;

            Console.WriteLine($"[DEVICE NOTIFICATION]: {e.Message}");
        }

        public static string ReadInput(string prompt)
        {
            Console.Write($"{prompt}: ");
            return Console.ReadLine() ?? string.Empty;
        }

        public static bool ReadBool(string prompt)
        {
            while (true)
            {
                Console.Write($"{prompt} (y/n): ");
                string input = Console.ReadLine()?.Trim().ToLower() ?? "";

                if (input == "y" || input == "yes") return true;
                if (input == "n" || input == "no") return false;

                PrintInfo("Invalid input. Please explicitly type 'y' or 'n'.");
            }
        }
    }
}