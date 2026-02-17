using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Restaurant.BLL.Models;

namespace Restaurant.BLL.Interfaces
{
    public interface IDishRepository : IRepository<Dish>
    {
        // Наразі не має унікальних методів, але може мати в майбутньому
    }
}
