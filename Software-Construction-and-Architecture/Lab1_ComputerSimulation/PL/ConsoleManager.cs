using System;
using HardwareSim.BLL.Events;

namespace HardwareSim.PL
{
    public static class ConsoleManager
    {
        public static void PrintHeader(string title)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n=== {title} ===");
            Console.ResetColor();
        }

        public static void PrintInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"[INFO] {message}");
            Console.ResetColor();
        }

        // Event handler method that matches the signature required by BLL events 
        public static void HandleDeviceNotification(object sender, HardwareEventArgs e)
        {
            if (e.Message.Contains("Failed") || e.Message.Contains("shutting down") || e.Message.Contains("died"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ALERT] {e.Message}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"[SYSTEM] {e.Message}");
            }
            Console.ResetColor();
        }

        public static void Pause()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
}