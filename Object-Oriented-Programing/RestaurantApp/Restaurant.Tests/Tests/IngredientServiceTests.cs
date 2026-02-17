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
    public class IngredientServiceTests
    {
        [TestMethod]
        public void AddIngredient_ShouldAddIngredientToRepository()
        {
            // Arrange
            var fakeRepo = new FakeIngredientRepository();
            var fakeDishRepo = new FakeDishRepository();
            var service = new IngredientService(fakeRepo, fakeDishRepo);

            // Act
            var result = service.AddIngredient("Картопля", "кг");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, fakeRepo.Data.Count);
            Assert.AreEqual("Картопля", fakeRepo.Data[0].Name);
            Assert.IsTrue(result.Id > 0);
        }

        [TestMethod]
        public void GetAllIngredients_ShouldReturnAllItems()
        {
            // Arrange
            var fakeRepo = new FakeIngredientRepository();
            fakeRepo.Data.Add(new Ingredient { Id = 1, Name = "A" });
            fakeRepo.Data.Add(new Ingredient { Id = 2, Name = "B" });
            var service = new IngredientService(fakeRepo, new FakeDishRepository());

            // Act
            var result = service.GetAllIngredients();

            // Assert
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public void GetIngredientById_ShouldReturnCorrectItem_WhenExists()
        {
            // Arrange
            var fakeRepo = new FakeIngredientRepository();
            fakeRepo.Data.Add(new Ingredient { Id = 99, Name = "TestItem" });
            var service = new IngredientService(fakeRepo, new FakeDishRepository());

            // Act
            var result = service.GetIngredientById(99);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("TestItem", result.Name);
        }

        [TestMethod]
        public void GetIngredientById_ShouldThrowNotFound_WhenIdDoesNotExist()
        {
            // Arrange
            var fakeRepo = new FakeIngredientRepository();
            var service = new IngredientService(fakeRepo, new FakeDishRepository());

            // Act & Assert
            // Перевіряємо, що кидається помилка NotFoundException
            Assert.ThrowsException<NotFoundException>(() => service.GetIngredientById(999));
        }

        [TestMethod]
        public void SearchIngredients_ShouldReturnMatches()
        {
            // Arrange
            var fakeRepo = new FakeIngredientRepository();
            fakeRepo.Data.Add(new Ingredient { Name = "Картопля" });
            fakeRepo.Data.Add(new Ingredient { Name = "Картон" }); // Схоже слово
            fakeRepo.Data.Add(new Ingredient { Name = "Цибуля" });
            var service = new IngredientService(fakeRepo, new FakeDishRepository());

            // Act
            var result = service.SearchIngredients("Кар");

            // Assert
            Assert.AreEqual(2, result.Count()); // Має знайти Картоплю і Картон
            Assert.IsFalse(result.Any(i => i.Name == "Цибуля"));
        }

        [TestMethod]
        public void UpdateIngredient_ShouldUpdateNameAndUnit()
        {
            // Arrange
            var fakeRepo = new FakeIngredientRepository();
            var service = new IngredientService(fakeRepo, new FakeDishRepository());
            fakeRepo.Data.Add(new Ingredient { Id = 1, Name = "OldName", Unit = "OldUnit" });

            // Act
            var updateModel = new Ingredient { Id = 1, Name = "NewName", Unit = "NewUnit" };
            service.UpdateIngredient(updateModel);

            // Assert
            var updated = fakeRepo.GetById(1);
            Assert.IsNotNull(updated);
            Assert.AreEqual("NewName", updated!.Name);
            Assert.AreEqual("NewUnit", updated.Unit);
        }

        [TestMethod]
        public void DeleteIngredient_ShouldDelete_WhenNotUsedInDish()
        {
            // Arrange
            var fakeRepo = new FakeIngredientRepository();
            var service = new IngredientService(fakeRepo, new FakeDishRepository());
            fakeRepo.Data.Add(new Ingredient { Id = 10 });

            // Act
            service.DeleteIngredient(10);

            // Assert
            Assert.AreEqual(0, fakeRepo.Data.Count);
        }

        [TestMethod]
        public void DeleteIngredient_ShouldThrowException_WhenUsedInDish()
        {
            // Arrange
            var fakeIngRepo = new FakeIngredientRepository();
            var fakeDishRepo = new FakeDishRepository();
            var service = new IngredientService(fakeIngRepo, fakeDishRepo);

            fakeIngRepo.Data.Add(new Ingredient { Id = 5 });
            fakeDishRepo.Data.Add(new Dish
            {
                Id = 1,
                IngredientIds = new List<int> { 5 } // Інгредієнт використовується!
            });

            // Act & Assert
            Assert.ThrowsException<EntityInUseException>(() => service.DeleteIngredient(5));
        }
    }
}