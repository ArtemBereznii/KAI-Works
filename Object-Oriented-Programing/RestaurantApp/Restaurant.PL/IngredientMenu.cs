using Restaurant.BLL.Exceptions;
using Restaurant.BLL.Models;
using Restaurant.BLL.Services;
using System;
using System.Linq;

namespace Restaurant.PL
{
    public class IngredientMenu
    {
        private readonly IngredientService _ingredientService;

        public IngredientMenu(IngredientService ingredientService)
        {
            _ingredientService = ingredientService;
        }

        public void Show()
        {
            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine("--- Управління Інгредієнтами ---");
                Console.WriteLine("1. Додати інгредієнт");
                Console.WriteLine("2. Змінити інгредієнт");
                Console.WriteLine("3. Видалити інгредієнт");
                Console.WriteLine("4. Переглянути всі інгредієнти");
                Console.WriteLine("5. Пошук інгредієнтів");
                Console.WriteLine("0. Повернутися до головного меню");

                string choice = Console.ReadLine();
                try
                {
                    switch (choice)
                    {
                        case "1": AddIngredient(); break;
                        case "2": UpdateIngredient(); break;
                        case "3": DeleteIngredient(); break;
                        case "4": ListAllIngredients(); break;
                        case "5": SearchIngredients(); break;
                        case "0": running = false; break;
                        default: Console.WriteLine("Невідома команда."); break;
                    }
                }
                catch (Exception ex)
                {
                    // Обробка винятків з BLL
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\nПОМИЛКА: {ex.Message}");
                    Console.ResetColor();
                }

                if (running) ConsoleHelpers.PressEnterToContinue();
            }
        }

        private void AddIngredient()
        {
            Console.WriteLine("\n--- Додавання інгредієнта ---");
            string name = ConsoleHelpers.ReadString("Введіть назву");
            string unit = ConsoleHelpers.ReadString("Введіть одиницю виміру (г, мл, шт)");

            var ingredient = _ingredientService.AddIngredient(name, unit);
            Console.WriteLine($"\nУспішно додано інгредієнт: {ingredient.Name} (ID: {ingredient.Id})");
        }

        private void ListAllIngredients()
        {
            Console.WriteLine("\n--- Список всіх інгредієнтів ---");
            var ingredients = _ingredientService.GetAllIngredients();
            if (!ingredients.Any())
            {
                Console.WriteLine("Список порожній.");
                return;
            }
            foreach (var ing in ingredients)
            {
                Console.WriteLine($"ID: {ing.Id} | Назва: {ing.Name} | Одиниця: {ing.Unit}");
            }
        }

        private void UpdateIngredient()
        {
            Console.WriteLine("\n--- Зміна інгредієнта ---");
            ListAllIngredients();
            int id = ConsoleHelpers.ReadInt("\nВведіть ID інгредієнта для зміни", 1);

            var ingredient = _ingredientService.GetIngredientById(id);

            Console.WriteLine($"Поточна назва: {ingredient.Name}.");
            string newName = ConsoleHelpers.ReadString("Введіть нову назву (або Enter, щоб лишити)");
            if (!string.IsNullOrWhiteSpace(newName))
            {
                ingredient.Name = newName;
            }

            Console.WriteLine($"Поточна одиниця: {ingredient.Unit}.");
            string newUnit = ConsoleHelpers.ReadString("Введіть нову одиницю (або Enter, щоб лишити)");
            if (!string.IsNullOrWhiteSpace(newUnit))
            {
                ingredient.Unit = newUnit;
            }

            _ingredientService.UpdateIngredient(ingredient);
            Console.WriteLine("Інгредієнт успішно оновлено.");
        }

        private void DeleteIngredient()
        {
            Console.WriteLine("\n--- Видалення інгредієнта ---");
            ListAllIngredients();
            int id = ConsoleHelpers.ReadInt("\nВведіть ID інгредієнта для видалення", 1);

            // BLL сам перевірить, чи можна видаляти, і згенерує виняток
            _ingredientService.DeleteIngredient(id);
            Console.WriteLine("Інгредієнт успішно видалено.");
        }

        private void SearchIngredients()
        {
            Console.WriteLine("\n--- Пошук інгредієнтів ---");
            string keyword = ConsoleHelpers.ReadString("Введіть ключове слово для пошуку");

            var ingredients = _ingredientService.SearchIngredients(keyword);

            if (!ingredients.Any())
            {
                Console.WriteLine("Нічого не знайдено.");
                return;
            }
            Console.WriteLine("Результати пошуку:");
            foreach (var ing in ingredients)
            {
                Console.WriteLine($"ID: {ing.Id} | Назва: {ing.Name} | Одиниця: {ing.Unit}");
            }
        }
    }
}