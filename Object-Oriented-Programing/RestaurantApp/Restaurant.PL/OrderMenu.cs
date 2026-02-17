using Restaurant.BLL.Exceptions;
using Restaurant.BLL.Services;
using System;
using System.Linq;

namespace Restaurant.PL
{
    public class OrderMenu
    {
        private readonly OrderService _orderService;
        private readonly DishService _dishService;

        public OrderMenu(OrderService orderService, DishService dishService)
        {
            _orderService = orderService;
            _dishService = dishService;
        }

        public void Show()
        {
            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine("--- Управління Замовленнями ---");
                Console.WriteLine("1. Створити нове замовлення");
                Console.WriteLine("2. Змінити замовлення");
                Console.WriteLine("3. Видалити замовлення");
                Console.WriteLine("4. Переглянути всі замовлення");
                Console.WriteLine("5. Переглянути інформацію про замовлення");
                Console.WriteLine("6. Пошук замовлень");
                Console.WriteLine("0. Повернутися до головного меню");

                string choice = Console.ReadLine();
                try
                {
                    switch (choice)
                    {
                        case "1": CreateOrder(); break;
                        case "2": UpdateOrder(); break;
                        case "3": DeleteOrder(); break;
                        case "4": ListAllOrders(); break;
                        case "5": ShowOrderDetails(); break;
                        case "6": SearchOrders(); break;
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

        private void CreateOrder()
        {
            Console.WriteLine("\n--- Створення замовлення ---");
            int tableNum = ConsoleHelpers.ReadInt("Введіть номер столика", 1);
            var order = _orderService.CreateOrder(tableNum);
            Console.WriteLine($"\nСтворено замовлення ID: {order.Id} для столика {order.TableNumber}.");
            Console.WriteLine("Тепер додайте страви до замовлення.");
            ManageOrderItems(order.Id);
        }

        private void ListAllOrders()
        {
            Console.WriteLine("\n--- Список всіх замовлень ---");
            var orders = _orderService.GetAllOrders();
            if (!orders.Any())
            {
                Console.WriteLine("Список порожній.");
                return;
            }
            foreach (var order in orders)
            {
                Console.WriteLine($"ID: {order.Id} | Столик: {order.TableNumber} | Час: {order.OrderTime} | Сума: {order.TotalPrice:C}");
            }
        }

        private void ShowOrderDetails()
        {
            Console.WriteLine("\n--- Деталі замовлення ---");
            ListAllOrders();
            int id = ConsoleHelpers.ReadInt("\nВведіть ID замовлення для перегляду", 1);
            var order = _orderService.GetOrderById(id);

            Console.WriteLine($"\nЗамовлення ID: {order.Id}");
            Console.WriteLine($"Столик: {order.TableNumber}");
            Console.WriteLine($"Час: {order.OrderTime}");

            Console.WriteLine("Позиції в замовленні:");
            if (!order.Items.Any())
            {
                Console.WriteLine(" (замовлення порожнє)");
            }
            else
            {
                foreach (var item in order.Items)
                {
                    try
                    {
                        var dish = _dishService.GetDishById(item.DishId);
                        Console.WriteLine($" - {dish.Name} (ID: {dish.Id})");
                        Console.WriteLine($"   Кількість: {item.Quantity}");
                        Console.WriteLine($"   Ціна за од.: {item.ItemPrice:C}");
                        Console.WriteLine($"   Разом: {(item.Quantity * item.ItemPrice):C}");
                    }
                    catch (NotFoundException)
                    {
                        Console.WriteLine($" - (Страва з ID={item.DishId} була видалена)");
                    }
                }
            }
            Console.WriteLine("---------------------------------");
            Console.WriteLine($"ЗАГАЛЬНА СУМА: {order.TotalPrice:C}");
        }

        private void DeleteOrder()
        {
            Console.WriteLine("\n--- Видалення замовлення ---");
            ListAllOrders();
            int id = ConsoleHelpers.ReadInt("\nВведіть ID замовлення для видалення", 1);

            _orderService.DeleteOrder(id);
            Console.WriteLine("Замовлення успішно видалено.");
        }

        private void SearchOrders()
        {
            Console.WriteLine("\n--- Пошук замовлень ---");
            string keyword = ConsoleHelpers.ReadString("Введіть номер столика для пошуку");

            var orders = _orderService.SearchOrders(keyword);

            if (!orders.Any())
            {
                Console.WriteLine("Нічого не знайдено.");
                return;
            }
            Console.WriteLine("Результати пошуку:");
            foreach (var order in orders)
            {
                Console.WriteLine($"ID: {order.Id} | Столик: {order.TableNumber} | Час: {order.OrderTime} | Сума: {order.TotalPrice:C}");
            }
        }

        private void UpdateOrder()
        {
            Console.WriteLine("\n--- Зміна замовлення ---");
            ListAllOrders();
            int id = ConsoleHelpers.ReadInt("\nВведіть ID замовлення для зміни", 1);
            var order = _orderService.GetOrderById(id); // Перевірка, чи замовлення існує

            Console.Clear();
            Console.WriteLine($"Обрано замовлення ID: {id} (Столик: {order.TableNumber})");
            Console.WriteLine("1. Змінити номер столика");
            Console.WriteLine("2. Змінити кількість страв (додати/видалити)");
            Console.WriteLine("0. Повернутися");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    int newTable = ConsoleHelpers.ReadInt("Введіть новий номер столика", 1);
                    _orderService.ChangeTableNumber(id, newTable);
                    Console.WriteLine("Номер столика оновлено.");
                    break;
                case "2":
                    ManageOrderItems(id);
                    break;
                case "0":
                    break;
            }
        }

        // Внутрішній метод для реалізації п. 76
        private void ManageOrderItems(int orderId)
        {
            Console.Clear();
            Console.WriteLine($"--- Редагування позицій замовлення ID: {orderId} ---");
            ShowOrderDetailsInternal(orderId); // Показуємо поточний стан замовлення

            Console.WriteLine("\n1. Додати/збільшити кількість страви");
            Console.WriteLine("2. Змінити кількість страви");
            Console.WriteLine("3. Видалити страву з замовлення");
            Console.WriteLine("0. Завершити редагування");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    Console.WriteLine("\n--- Доступні страви ---");
                    var dishes = _dishService.GetAllDishes();
                    foreach (var d in dishes)
                    {
                        Console.WriteLine($"ID: {d.Id} | {d.Name} | {d.Price:C}");
                    }
                    int dishIdToAdd = ConsoleHelpers.ReadInt("Введіть ID страви", 1);
                    int qtyToAdd = ConsoleHelpers.ReadInt("Введіть кількість", 1);
                    _orderService.AddDishToOrder(orderId, dishIdToAdd, qtyToAdd);
                    Console.WriteLine("Позицію додано/оновлено.");
                    break;
                case "2":
                    int dishIdToChange = ConsoleHelpers.ReadInt("Введіть ID страви, кількість якої треба змінити", 1);
                    int newQty = ConsoleHelpers.ReadInt("Введіть НОВУ кількість (0 для видалення)", 0);
                    _orderService.ChangeDishQuantityInOrder(orderId, dishIdToChange, newQty);
                    Console.WriteLine("Кількість змінено.");
                    break;
                case "3":
                    int dishIdToRemove = ConsoleHelpers.ReadInt("Введіть ID страви, яку треба видалити", 1);
                    _orderService.ChangeDishQuantityInOrder(orderId, dishIdToRemove, 0); // Встановлення кількості 0 = видалення
                    Console.WriteLine("Позицію видалено.");
                    break;
                case "0":
                    return;
            }
        }

        private void ShowOrderDetailsInternal(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            Console.WriteLine("\n--- Поточні позиції ---");
            if (!order.Items.Any())
            {
                Console.WriteLine(" (замовлення порожнє)");
            }
            else
            {
                foreach (var item in order.Items)
                {
                    var dish = _dishService.GetDishById(item.DishId); // Може кинути виняток, якщо страва видалена
                    Console.WriteLine($" - {dish.Name} (ID: {item.DishId}) | Кількість: {item.Quantity} | Разом: {(item.Quantity * item.ItemPrice):C}");
                }
            }
            Console.WriteLine($"ЗАГАЛЬНА СУМА: {order.TotalPrice:C}");
        }
    }
}