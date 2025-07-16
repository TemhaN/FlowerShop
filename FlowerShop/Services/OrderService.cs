using Microsoft.EntityFrameworkCore;
using FlowerShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlowerShop.Services
{
    public class OrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly CartService _cartService;

        public OrderService(ApplicationDbContext context, CartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }

        public async Task CreateOrder(OrderDto orderDto)
        {
            var cart = await _cartService.GetCart(orderDto.UserId);
            if (!cart.CartItems.Any())
            {
                throw new InvalidOperationException("Корзина пуста");
            }

            // Проверка складских запасов
            foreach (var item in cart.CartItems)
            {
                if (item.BouquetId.HasValue)
                {
                    var bouquet = await _context.Bouquets.FindAsync(item.BouquetId);
                    if (bouquet == null || bouquet.Stock < item.Quantity)
                    {
                        throw new InvalidOperationException($"Недостаточно товара {bouquet?.Name ?? "букет"} на складе");
                    }
                    bouquet.Stock -= item.Quantity;
                }
                else if (item.FlowerId.HasValue)
                {
                    var flower = await _context.Flowers.FindAsync(item.FlowerId);
                    if (flower == null || flower.Stock < item.Quantity)
                    {
                        throw new InvalidOperationException($"Недостаточно товара {flower?.Name ?? "цветок"} на складе");
                    }
                    flower.Stock -= item.Quantity;
                }
                else if (item.ToyId.HasValue)
                {
                    var toy = await _context.Toys.FindAsync(item.ToyId);
                    if (toy == null || toy.Stock < item.Quantity)
                    {
                        throw new InvalidOperationException($"Недостаточно товара {toy?.Name ?? "игрушка"} на складе");
                    }
                    toy.Stock -= item.Quantity;
                }
            }

            var order = new Order
            {
                UserId = orderDto.UserId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = cart.CartItems.Sum(ci => ci.Quantity * (ci.Bouquet?.Price ?? ci.Flower?.Price ?? ci.Toy?.Price ?? 0)),
                OrderItems = cart.CartItems.Select(ci => new OrderItem
                {
                    BouquetId = ci.BouquetId,
                    FlowerId = ci.FlowerId,
                    ToyId = ci.ToyId,
                    Quantity = ci.Quantity,
                    Price = ci.Bouquet?.Price ?? ci.Flower?.Price ?? ci.Toy?.Price ?? 0
                }).ToList()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Очищаем корзину
            await _cartService.ClearCart(orderDto.UserId);
        }

        public async Task<List<OrderDto>> GetUserOrders(int userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Bouquet)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Flower)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Toy)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        Id = oi.Id,
                        BouquetId = oi.BouquetId,
                        FlowerId = oi.FlowerId,
                        ToyId = oi.ToyId,
                        Quantity = oi.Quantity,
                        Price = oi.Price,
                        ItemName = oi.BouquetId.HasValue ? oi.Bouquet.Name :
                                   oi.FlowerId.HasValue ? oi.Flower.Name :
                                   oi.ToyId.HasValue ? oi.Toy.Name : "Неизвестный товар"
                    }).ToList()
                })
                .ToListAsync();
        }
    }
}