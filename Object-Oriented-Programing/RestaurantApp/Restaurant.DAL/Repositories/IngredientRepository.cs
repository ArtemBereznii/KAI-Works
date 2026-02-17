using Restaurant.BLL.Interfaces;
using Restaurant.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Restaurant.DAL.Repositories
{
    public class IngredientRepository : FileRepositoryBase<Ingredient>, IIngredientRepository
    {
        public IngredientRepository() : base("ingredients.json") { }

        // Метод пошуку
        public IEnumerable<Ingredient> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return GetAll();
            }

            return _entities.Where(i => i.Name != null && i.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        }
    }
}