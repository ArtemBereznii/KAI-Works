using Restaurant.BLL.Models;
using System.Collections.Generic;

namespace Restaurant.BLL.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        IEnumerable<Order> Search(string keyword);
    }
}