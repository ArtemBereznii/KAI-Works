using BLL.Services;
using DAL;
using PL;
using System;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            // 1. Створюємо DAL
            var context = new EntityContext();

            // 2. Створюємо BLL і передаємо йому DAL
            var studentService = new StudentService(context);

            // 3. Створюємо PL і передаємо йому BLL
            var menu = new Menu(studentService);

            // 4. Запускаємо меню (єдиний виклик)
            menu.MainMenu();
        }
    }
}