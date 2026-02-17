using Restaurant.BLL.Services;
using System;

namespace Restaurant.PL
{
    public class MainMenu
    {
        private readonly IngredientMenu _ingredientMenu;
        private readonly DishMenu _dishMenu;
        private readonly OrderMenu _orderMenu;

        public MainMenu(IngredientService ingredientService, DishService dishService, OrderService orderService)
        {
            _ingredientMenu = new IngredientMenu(ingredientService);
            _dishMenu = new DishMenu(dishService, ingredientService);
            _orderMenu = new OrderMenu(orderService, dishService);
        }

        public void Run()
        {
            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine("--- Система Управління Рестораном ---");
                Console.WriteLine("1. Управління Інгредієнтами");
                Console.WriteLine("2. Управління Стравами");
                Console.WriteLine("3. Управління Замовленнями");
                Console.WriteLine("0. Вихід");

                Console.Write("\nВаш вибір: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        _ingredientMenu.Show();
                        break;
                    case "2":
                        _dishMenu.Show();
                        break;
                    case "3":
                        _orderMenu.Show();
                        break;
                    case "0":
                        running = false;
                        Console.WriteLine("Дякуємо за використання системи. До побачення!");
                        break;
                    default:
                        Console.WriteLine("Невідома команда. Спробуйте ще раз.");
                        ConsoleHelpers.PressEnterToContinue();
                        break;
                }
            }
        }
    }
}