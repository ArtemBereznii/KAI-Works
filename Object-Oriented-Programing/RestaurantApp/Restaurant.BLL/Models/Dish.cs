using System;
using System.Collections.Generic;

namespace Restaurant.BLL.Models
{
    public class Dish : IEntity
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public TimeSpan PreparationTime { get; set; }

        // Зберігаємо тільки ID інгредієнтів
        public List<int> IngredientIds { get; set; } = new List<int>();
    }
}