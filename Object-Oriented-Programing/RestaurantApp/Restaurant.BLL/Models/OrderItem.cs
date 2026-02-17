namespace Restaurant.BLL.Models
{
    public class OrderItem
    {
        public int DishId { get; set; }
        public int Quantity { get; set; }
        public decimal ItemPrice { get; set; } // Ціна за одиницю на момент замовлення
    }
}