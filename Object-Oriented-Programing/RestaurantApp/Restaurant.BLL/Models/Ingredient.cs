namespace Restaurant.BLL.Models
{
    public class Ingredient : IEntity
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Unit { get; set; } // напр. "г", "мл", "шт"
    }
}