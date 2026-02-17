using System;
using System.Globalization;
using Restaurant.BLL.Interfaces;
using Restaurant.BLL.Services;
using Restaurant.DAL.Repositories;
using Restaurant.PL;

namespace Restaurant.PL
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            CultureInfo.CurrentCulture = new CultureInfo("uk-UA");

            Console.Title = "Restaurant Management System";

            IIngredientRepository ingredientRepo = new IngredientRepository();
            IDishRepository dishRepo = new DishRepository();
            IOrderRepository orderRepo = new OrderRepository();

            // IngredientService потребує DishRepository для перевірки видалення
            IngredientService ingredientService = new IngredientService(ingredientRepo, dishRepo);

            // DishService потребує IngredientRepository для перевірки додавання інгредієнта
            DishService dishService = new DishService(dishRepo, ingredientRepo);

            // OrderService потребує DishRepository для отримання цін на страви
            OrderService orderService = new OrderService(orderRepo, dishRepo);

            MainMenu menu = new MainMenu(ingredientService, dishService, orderService);

            menu.Run();
        }
    }
}