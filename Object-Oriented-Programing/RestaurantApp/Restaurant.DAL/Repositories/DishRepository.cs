using Restaurant.BLL.Interfaces;
using Restaurant.BLL.Models;

namespace Restaurant.DAL.Repositories
{
    public class DishRepository : FileRepositoryBase<Dish>, IDishRepository
    {
        public DishRepository() : base("dishes.json") { }
    }
}