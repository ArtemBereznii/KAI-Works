using Restaurant.BLL.Exceptions;
using Restaurant.BLL.Interfaces;
using Restaurant.BLL.Models;
using System;
using System.Collections.Generic;

namespace Restaurant.BLL.Services
{
    public class DishService
    {
        private readonly IDishRepository _dishRepo;
        private readonly IIngredientRepository _ingredientRepo; // Потрібен для перевірок

        public DishService(IDishRepository dishRepo, IIngredientRepository ingredientRepo)
        {
            _dishRepo = dishRepo;
            _ingredientRepo = ingredientRepo;
        }

        public Dish AddDish(string name, decimal price, TimeSpan prepTime)
        {
            var dish = new Dish
            {
                Name = name,
                Price = price,
                PreparationTime = prepTime
            };
            _dishRepo.Add(dish);
            return dish;
        }

        public void DeleteDish(int id)
        {
            _dishRepo.Delete(id);
        }

        public void UpdateDishDetails(int dishId, string newName, decimal newPrice, TimeSpan newPrepTime)
        {
            var dish = GetDishById(dishId);
            dish.Name = newName;
            dish.Price = newPrice;
            dish.PreparationTime = newPrepTime;
            _dishRepo.Update(dish);
        }

        public void AddIngredientToDish(int dishId, int ingredientId)
        {
            var dish = GetDishById(dishId);
            // Перевіряємо, чи існує такий інгредієнт
            var ingredient = _ingredientRepo.GetById(ingredientId);
            if (ingredient == null)
            {
                throw new NotFoundException($"Інгредієнт з Id={ingredientId} не знайдено.");
            }

            if (!dish.IngredientIds.Contains(ingredientId))
            {
                dish.IngredientIds.Add(ingredientId);
                _dishRepo.Update(dish);
            }
        }

        public void RemoveIngredientFromDish(int dishId, int ingredientId)
        {
            var dish = GetDishById(dishId);
            if (dish.IngredientIds.Contains(ingredientId))
            {
                dish.IngredientIds.Remove(ingredientId);
                _dishRepo.Update(dish);
            }
        }

        public Dish GetDishById(int id)
        {
            var dish = _dishRepo.GetById(id);
            if (dish == null)
            {
                throw new NotFoundException($"Страва з Id={id} не знайдена.");
            }
            return dish;
        }

        public IEnumerable<Dish> GetAllDishes()
        {
            return _dishRepo.GetAll();
        }
    }
}