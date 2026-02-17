using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Restaurant.BLL.Exceptions;
using Restaurant.BLL.Models;
using Restaurant.BLL.Services;
using Restaurant.Tests.Additional;

namespace Restaurant.Tests.Tests
{
    [TestClass]
    public class DishServiceTests
    {
        [TestMethod]
        public void AddDish_ShouldAddDishToRepository()
        {
            // Arrange
            var fakeDishRepo = new FakeDishRepository();
            var fakeIngRepo = new FakeIngredientRepository();
            var service = new DishService(fakeDishRepo, fakeIngRepo);

            // Act
            var result = service.AddDish("Борщ", 120.50m, TimeSpan.FromMinutes(45));

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, fakeDishRepo.Data.Count);
            Assert.AreEqual("Борщ", fakeDishRepo.Data[0].Name);
            Assert.AreEqual(120.50m, fakeDishRepo.Data[0].Price);
        }

        [TestMethod]
        public void GetDishById_ShouldThrowNotFound_WhenIdDoesNotExist()
        {
            // Arrange
            var service = new DishService(new FakeDishRepository(), new FakeIngredientRepository());

            // Act & Assert
            Assert.ThrowsException<NotFoundException>(() => service.GetDishById(999));
        }

        [TestMethod]
        public void UpdateDishDetails_ShouldUpdateFields()
        {
            // Arrange
            var fakeDishRepo = new FakeDishRepository();
            var service = new DishService(fakeDishRepo, new FakeIngredientRepository());

            // Додаємо початкову страву
            fakeDishRepo.Data.Add(new Dish
            {
                Id = 1,
                Name = "Суп",
                Price = 50m,
                PreparationTime = TimeSpan.FromMinutes(10)
            });

            // Act
            service.UpdateDishDetails(1, "Суп Харчо", 85m, TimeSpan.FromMinutes(20));

            // Assert
            var updated = fakeDishRepo.GetById(1);
            Assert.AreEqual("Суп Харчо", updated.Name);
            Assert.AreEqual(85m, updated.Price);
            Assert.AreEqual(20, updated.PreparationTime.TotalMinutes);
        }

        [TestMethod]
        public void AddIngredientToDish_ShouldAddId_WhenIngredientExists()
        {
            // Arrange
            var fakeDishRepo = new FakeDishRepository();
            var fakeIngRepo = new FakeIngredientRepository();
            var service = new DishService(fakeDishRepo, fakeIngRepo);

            // Готуємо дані: інгредієнт має існувати, щоб його можна було додати
            fakeIngRepo.Data.Add(new Ingredient { Id = 10, Name = "Сіль" });
            fakeDishRepo.Data.Add(new Dish { Id = 1, Name = "Салат" });

            // Act
            service.AddIngredientToDish(1, 10); // Додаємо Сіль у Салат

            // Assert
            var dish = fakeDishRepo.GetById(1);
            Assert.IsTrue(dish.IngredientIds.Contains(10));
        }

        [TestMethod]
        public void AddIngredientToDish_ShouldThrowNotFound_WhenIngredientDoesNotExist()
        {
            // Arrange
            var fakeDishRepo = new FakeDishRepository();
            var fakeIngRepo = new FakeIngredientRepository(); // Порожній репозиторій інгредієнтів
            var service = new DishService(fakeDishRepo, fakeIngRepo);

            fakeDishRepo.Data.Add(new Dish { Id = 1, Name = "Салат" });

            // Act & Assert
            // Спроба додати неіснуючий інгредієнт (ID 99) має викликати помилку
            Assert.ThrowsException<NotFoundException>(() => service.AddIngredientToDish(1, 99));
        }

        [TestMethod]
        public void RemoveIngredientFromDish_ShouldRemoveId()
        {
            // Arrange
            var fakeDishRepo = new FakeDishRepository();
            var service = new DishService(fakeDishRepo, new FakeIngredientRepository());

            fakeDishRepo.Data.Add(new Dish
            {
                Id = 1,
                IngredientIds = new List<int> { 10, 20 }
            });

            // Act
            service.RemoveIngredientFromDish(1, 10);

            // Assert
            var dish = fakeDishRepo.GetById(1);
            Assert.IsFalse(dish.IngredientIds.Contains(10)); // 10 видалено
            Assert.IsTrue(dish.IngredientIds.Contains(20));  // 20 залишилось
        }

        [TestMethod]
        public void DeleteDish_ShouldRemoveFromRepo()
        {
            // Arrange
            var fakeDishRepo = new FakeDishRepository();
            var service = new DishService(fakeDishRepo, new FakeIngredientRepository());
            fakeDishRepo.Data.Add(new Dish { Id = 1 });

            // Act
            service.DeleteDish(1);

            // Assert
            Assert.AreEqual(0, fakeDishRepo.Data.Count);
        }
    }
}