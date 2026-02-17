using Restaurant.BLL.Exceptions;
using Restaurant.BLL.Interfaces;
using Restaurant.BLL.Models;
using System.Collections.Generic;
using System.Linq;

namespace Restaurant.BLL.Services
{
    public class IngredientService
    {
        private readonly IIngredientRepository _ingredientRepo;
        private readonly IDishRepository _dishRepo;

        public IngredientService(IIngredientRepository ingredientRepo, IDishRepository dishRepo)
        {
            _ingredientRepo = ingredientRepo;
            _dishRepo = dishRepo;
        }

        public Ingredient AddIngredient(string name, string unit)
        {
            var ingredient = new Ingredient { Name = name, Unit = unit };
            _ingredientRepo.Add(ingredient);
            return ingredient;
        }

        public void UpdateIngredient(Ingredient ingredient)
        {
            _ingredientRepo.Update(ingredient);
        }

        public void DeleteIngredient(int id)
        {
            // Перевірка, чи не використовується інгредієнт 
            var allDishes = _dishRepo.GetAll();
            bool isUsed = allDishes.Any(d => d.IngredientIds.Contains(id));

            if (isUsed)
            {
                throw new EntityInUseException("Неможливо видалити інгредієнт, оскільки він використовується у страві.");
            }

            _ingredientRepo.Delete(id);
        }

        public IEnumerable<Ingredient> GetAllIngredients()
        {
            return _ingredientRepo.GetAll();
        }

        public Ingredient GetIngredientById(int id)
        {
            var ingredient = _ingredientRepo.GetById(id);
            if (ingredient == null)
            {
                throw new NotFoundException($"Інгредієнт з Id={id} не знайдено.");
            }
            return ingredient;
        }

        public IEnumerable<Ingredient> SearchIngredients(string keyword)
        {
            return _ingredientRepo.Search(keyword);
        }
    }
}