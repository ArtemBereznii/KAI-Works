using Microsoft.VisualStudio.TestTools.UnitTesting;
using Restaurant.BLL.Models;
using Restaurant.BLL.Services;
using Restaurant.Tests.Additional;
using System;
using System.Linq;

namespace Restaurant.Tests.Tests
{
    [TestClass]
    public class OrderServiceTests
    {
        [TestMethod]
        public void CreateOrder_ShouldInitializeCorrectly()
        {
            // Arrange
            var orderRepo = new FakeOrderRepository();
            var dishRepo = new FakeDishRepository();
            var service = new OrderService(orderRepo, dishRepo);

            // Act
            var order = service.CreateOrder(5); // Стіл №5

            // Assert
            Assert.IsNotNull(order);
            Assert.AreEqual(5, order.TableNumber);
            Assert.AreEqual(0, order.TotalPrice); // Поки що 0
            Assert.IsNotNull(order.Items);
        }

        [TestMethod]
        public void AddDishToOrder_ShouldCalculateTotalPriceCorrectly()
        {
            // Arrange
            var orderRepo = new FakeOrderRepository();
            var dishRepo = new FakeDishRepository();
            var service = new OrderService(orderRepo, dishRepo);

            // Готуємо дані: страву ціною 100 грн
            dishRepo.Add(new Dish { Id = 1, Name = "Стейк", Price = 100m });

            // Створюємо замовлення
            var order = service.CreateOrder(1);

            // Act
            // Додаємо 2 стейки (2 * 100 = 200)
            service.AddDishToOrder(order.Id, 1, 2);

            // Assert
            var savedOrder = orderRepo.GetById(order.Id);
            Assert.AreEqual(1, savedOrder.Items.Count);
            Assert.AreEqual(200m, savedOrder.TotalPrice); // Перевірка автоматичного розрахунку
        }

        [TestMethod]
        public void ChangeDishQuantity_ShouldUpdateTotal()
        {
            // Arrange
            var orderRepo = new FakeOrderRepository();
            var dishRepo = new FakeDishRepository();
            var service = new OrderService(orderRepo, dishRepo);

            dishRepo.Add(new Dish { Id = 1, Name = "Кава", Price = 50m });
            var order = service.CreateOrder(1);
            service.AddDishToOrder(order.Id, 1, 1); // Спочатку 1 кава (50 грн)

            // Act
            service.ChangeDishQuantityInOrder(order.Id, 1, 3); // Змінюємо на 3 кави (150 грн)

            // Assert
            var savedOrder = orderRepo.GetById(order.Id);
            Assert.AreEqual(3, savedOrder.Items.First().Quantity);
            Assert.AreEqual(150m, savedOrder.TotalPrice);
        }
    }
}