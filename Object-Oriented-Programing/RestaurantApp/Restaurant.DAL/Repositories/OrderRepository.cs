using Restaurant.BLL.Interfaces;
using Restaurant.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Restaurant.DAL.Repositories
{
    public class OrderRepository : FileRepositoryBase<Order>, IOrderRepository
    {
        public OrderRepository() : base("orders.json") { }

        // Шукаємо за номером столика
        public IEnumerable<Order> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return GetAll();
            }

            if (int.TryParse(keyword, out int tableNumber))
            {
                return _entities.Where(o => o.TableNumber == tableNumber);
            }

            return new List<Order>();
        }
    }
}