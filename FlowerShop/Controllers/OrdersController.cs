using Microsoft.AspNetCore.Mvc;
using FlowerShop.Models;
using FlowerShop.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FlowerShop.Controllers
{
    public class OrdersController : Controller
    {
        private readonly OrderService _orderService;
        private readonly CartService _cartService;

        public OrdersController(OrderService orderService, CartService cartService)
        {
            _orderService = orderService;
            _cartService = cartService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var cart = await _cartService.GetCart(userId);
            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateOrder()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userCart = await _cartService.GetCart(userId);

            if (!userCart.CartItems.Any())
            {
                ModelState.AddModelError("", "Корзина пуста.");
                return View("Create", userCart);
            }

            var orderDto = new OrderDto
            {
                UserId = userId,
                OrderItems = userCart.CartItems.Select(ci => new OrderItemDto
                {
                    BouquetId = ci.BouquetId,
                    FlowerId = ci.FlowerId,
                    ToyId = ci.ToyId,
                    Quantity = ci.Quantity,
                    Price = ci.Bouquet?.Price ?? ci.Flower?.Price ?? ci.Toy?.Price ?? 0,
                    ItemName = ci.Bouquet?.Name ?? ci.Flower?.Name ?? ci.Toy?.Name ?? "Неизвестный товар"
                }).ToList(),
                TotalAmount = userCart.CartItems.Sum(ci => ci.Quantity * (ci.Bouquet?.Price ?? ci.Flower?.Price ?? ci.Toy?.Price ?? 0))
            };

            try
            {
                await _orderService.CreateOrder(orderDto);
                await _cartService.ClearCart(userId);
                return RedirectToAction("Index", "Home");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("Create", userCart);
            }
        }

        [Authorize]
        public async Task<IActionResult> MyOrders()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var orders = await _orderService.GetUserOrders(userId);
            return View(orders);
        }
    }
}