using Restaurant.BLL.Exceptions;
using Restaurant.BLL.Services;
using System;
using System.Linq;

namespace Restaurant.PL
{
    public class DishMenu
    {
        private readonly DishService _dishService;
        private readonly IngredientService _ingredientService;

        public DishMenu(DishService dishService, IngredientService ingredientService)
        {
            _dishService = dishService;
            _ingredientService = ingredientService;
        }

        public void Show()
        {
            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine("--- Управління Стравами ---");
                Console.WriteLine("1. Додати страву");
                Console.WriteLine("2. Змінити страву");
                Console.WriteLine("3. Видалити страву");
                Console.WriteLine("4. Управління інгредієнтами страви");
                Console.WriteLine("5. Переглянути всі страви");
                Console.WriteLine("6. Переглянути інформацію про страву");
                Console.WriteLine("0. Повернутися до головного меню");

                string choice = Console.ReadLine();
                try
                {
                    switch (choice)
                    {
                        case "1": AddDish(); break;
                        case "2": UpdateDish(); break;
                        case "3": DeleteDish(); break;
                        case "4": ManageDishIngredients(); break;
                        case "5": ListAllDishes(); break;
                        case "6": ShowDishDetails(); break;
                        case "0": running = false; break;
                        default: Console.WriteLine("Невідома команда."); break;
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\nПОМИЛКА: {ex.Message}");
                    Console.ResetColor();
                }

                if (running) ConsoleHelpers.PressEnterToContinue();
            }
        }

        private void AddDish()
        {
            Console.WriteLine("\n--- Додавання страви ---");
            string name = ConsoleHelpers.ReadString("Введіть назву страви");
            decimal price = ConsoleHelpers.ReadDecimal("Введіть ціну");
            TimeSpan prepTime = ConsoleHelpers.ReadTimeSpan("Введіть час приготування (у хвилинах)");

            var dish = _dishService.AddDish(name, price, prepTime);
            Console.WriteLine($"\nУспішно додано страву: {dish.Name} (ID: {dish.Id})");
        }

        private void ListAllDishes()
        {
            Console.WriteLine("\n--- Список всіх страв ---");
            var dishes = _dishService.GetAllDishes();
            if (!dishes.Any())
            {
                Console.WriteLine("Список порожній.");
                return;
            }
            foreach (var dish in dishes)
            {
                Console.WriteLine($"ID: {dish.Id} | Назва: {dish.Name} | Ціна: {dish.Price:C}");
            }
        }

        private void ShowDishDetails()
        {
            Console.WriteLine("\n--- Деталі страви ---");
            ListAllDishes();
            int id = ConsoleHelpers.ReadInt("\nВведіть ID страви для перегляду", 1);
            var dish = _dishService.GetDishById(id);

            Console.WriteLine($"\nНазва: {dish.Name} (ID: {dish.Id})");
            Console.WriteLine($"Ціна: {dish.Price:C}");
            Console.WriteLine($"Час приготування: {dish.PreparationTime.TotalMinutes} хв.");

            Console.WriteLine("Інгредієнти:");
            if (!dish.IngredientIds.Any())
            {
                Console.WriteLine(" (не вказано)");
            }
            else
            {
                foreach (var ingId in dish.IngredientIds)
                {
                    try
                    {
                        var ingredient = _ingredientService.GetIngredientById(ingId);
                        Console.WriteLine($" - {ingredient.Name} (ID: {ingredient.Id})");
                    }
                    catch (NotFoundException)
                    {
                        Console.WriteLine($" - (Інгредієнт з ID={ingId} було видалено)");
                    }
                }
            }
        }

        private void UpdateDish()
        {
            Console.WriteLine("\n--- Зміна страви ---");
            ListAllDishes();
            int id = ConsoleHelpers.ReadInt("\nВведіть ID страви для зміни", 1);

            var dish = _dishService.GetDishById(id);

            Console.WriteLine($"Поточна назва: {dish.Name}.");
            string newName = ConsoleHelpers.ReadString("Введіть нову назву (або Enter, щоб лишити)");
            if (string.IsNullOrWhiteSpace(newName)) newName = dish.Name;

            Console.WriteLine($"Поточна ціна: {dish.Price}.");
            decimal newPrice;
            Console.Write("Введіть нову ціну (або Enter, щоб лишити): ");
            string priceInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(priceInput))
            {
                newPrice = dish.Price;
            }
            else if (!decimal.TryParse(priceInput, out newPrice) || newPrice < 0)
            {
                Console.WriteLine("Некоректна ціна, зміну не застосовано.");
                newPrice = dish.Price;
            }

            Console.WriteLine($"Поточний час приготування: {dish.PreparationTime.TotalMinutes} хв.");
            TimeSpan newPrepTime;
            Console.Write("Введіть новий час (у хв, або Enter, щоб лишити): ");
            string timeInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(timeInput))
            {
                newPrepTime = dish.PreparationTime;
            }
            else if (int.TryParse(timeInput, out int minutes) && minutes > 0)
            {
                newPrepTime = TimeSpan.FromMinutes(minutes);
            }
            else
            {
                Console.WriteLine("Некоректний час, зміну не застосовано.");
                newPrepTime = dish.PreparationTime;
            }

            _dishService.UpdateDishDetails(id, newName, newPrice, newPrepTime);
            Console.WriteLine("Страва успішно оновлена.");
        }

        private void DeleteDish()
        {
            Console.WriteLine("\n--- Видалення страви ---");
            ListAllDishes();
            int id = ConsoleHelpers.ReadInt("\nВведіть ID страви для видалення", 1);

            _dishService.DeleteDish(id);
            Console.WriteLine("Страва успішно видалена.");
        }

        private void ManageDishIngredients()
        {
            Console.WriteLine("\n--- Управління інгредієнтами страви ---");
            ListAllDishes();
            int dishId = ConsoleHelpers.ReadInt("\nВведіть ID страви", 1);
            var dish = _dishService.GetDishById(dishId);

            Console.Clear();
            Console.WriteLine($"Обрана страва: {dish.Name}");
            Console.WriteLine("1. Додати інгредієнт до страви");
            Console.WriteLine("2. Видалити інгредієнт зі страви");
            Console.WriteLine("0. Повернутися");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    Console.WriteLine("\n--- Доступні інгредієнти ---");
                    var ingredients = _ingredientService.GetAllIngredients();
                    foreach (var ing in ingredients)
                    {
                        Console.WriteLine($"ID: {ing.Id} | Назва: {ing.Name}");
                    }
                    int ingIdToAdd = ConsoleHelpers.ReadInt("\nВведіть ID інгредієнта для додавання", 1);
                    _dishService.AddIngredientToDish(dishId, ingIdToAdd);
                    Console.WriteLine("Інгредієнт додано.");
                    break;
                case "2":
                    Console.WriteLine("\n--- Інгредієнти у страві ---");
                    // (Тут ми знову повинні отримати інгредієнти за їх ID)
                    if (!dish.IngredientIds.Any())
                    {
                        Console.WriteLine("У страви немає інгредієнтів.");
                        break;
                    }
                    foreach (var ingId in dish.IngredientIds)
                    {
                        var ing = _ingredientService.GetIngredientById(ingId);
                        Console.WriteLine($"ID: {ing.Id} | Назва: {ing.Name}");
                    }
                    int ingIdToRemove = ConsoleHelpers.ReadInt("\nВведіть ID інгредієнта для видалення", 1);
                    _dishService.RemoveIngredientFromDish(dishId, ingIdToRemove);
                    Console.WriteLine("Інгредієнт видалено.");
                    break;
                case "0":
                    break;
            }
        }
    }
}