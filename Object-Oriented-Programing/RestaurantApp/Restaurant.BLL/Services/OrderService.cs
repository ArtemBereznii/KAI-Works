using Restaurant.BLL.Exceptions;
using Restaurant.BLL.Interfaces;
using Restaurant.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Restaurant.BLL.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IDishRepository _dishRepo;

        public OrderService(IOrderRepository orderRepo, IDishRepository dishRepo)
        {
            _orderRepo = orderRepo;
            _dishRepo = dishRepo;
        }

        public Order CreateOrder(int tableNumber)
        {
            var order = new Order
            {
                TableNumber = tableNumber,
                OrderTime = DateTime.Now
            };
            _orderRepo.Add(order);
            return order;
        }

        public void DeleteOrder(int id)
        {
            _orderRepo.Delete(id);
        }

        public void AddDishToOrder(int orderId, int dishId, int quantity)
        {
            var order = GetOrderById(orderId);
            var dish = _dishRepo.GetById(dishId);
            if (dish == null)
            {
                throw new NotFoundException($"Страва з Id={dishId} не знайдена.");
            }

            var existingItem = order.Items.FirstOrDefault(item => item.DishId == dishId);

            if (existingItem != null)
            {
                // Якщо страва вже є, оновлюємо кількість
                existingItem.Quantity += quantity;
            }
            else
            {
                // Якщо страви нема, додаємо нову позицію
                order.Items.Add(new OrderItem
                {
                    DishId = dishId,
                    Quantity = quantity,
                    ItemPrice = dish.Price // Фіксуємо ціну на момент додавання
                });
            }
            _orderRepo.Update(order);
        }

        public void ChangeDishQuantityInOrder(int orderId, int dishId, int newQuantity)
        {
            var order = GetOrderById(orderId);
            var item = order.Items.FirstOrDefault(i => i.DishId == dishId);

            if (item == null)
            {
                throw new NotFoundException($"Страва з Id={dishId} відсутня у замовленні.");
            }

            if (newQuantity > 0)
            {
                item.Quantity = newQuantity;
            }
            else
            {
                // Якщо нова кількість 0 або менше, видаляємо позицію
                order.Items.Remove(item);
            }
            _orderRepo.Update(order);
        }

        public void ChangeTableNumber(int orderId, int newTableNumber)
        {
            var order = GetOrderById(orderId);
            order.TableNumber = newTableNumber;
            _orderRepo.Update(order);
        }

        public Order GetOrderById(int id)
        {
            var order = _orderRepo.GetById(id);
            if (order == null)
            {
                throw new NotFoundException($"Замовлення з Id={id} не знайдено.");
            }
            return order;
        }

        public IEnumerable<Order> GetAllOrders()
        {
            return _orderRepo.GetAll();
        }

        public IEnumerable<Order> SearchOrders(string keyword)
        {
            return _orderRepo.Search(keyword);
        }
    }
}