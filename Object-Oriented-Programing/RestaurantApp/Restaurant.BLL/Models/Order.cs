using System;
using System.Collections.Generic;
using System.Linq;

namespace Restaurant.BLL.Models
{
    public class Order : IEntity
    {
        public int Id { get; set; }
        public int TableNumber { get; set; }
        public DateTime OrderTime { get; set; }
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();

        // Автоматичне обчислення загальної вартості замовлення
        public decimal TotalPrice
        {
            get
            {
                return Items.Sum(item => item.ItemPrice * item.Quantity);
            }
        }
    }
}