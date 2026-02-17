using Restaurant.BLL.Models;
using System.Collections.Generic;

namespace Restaurant.BLL.Interfaces
{
    public interface IIngredientRepository : IRepository<Ingredient>
    {
        IEnumerable<Ingredient> Search(string keyword);
    }
}