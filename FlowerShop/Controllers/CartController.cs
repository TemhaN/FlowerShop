using Microsoft.AspNetCore.Mvc;
using FlowerShop.Models;
using FlowerShop.Services;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace FlowerShop.Controllers
{
    public class CartController : Controller
    {
        private readonly CartService _cartService;
        private readonly OrderService _orderService;

        public CartController(CartService cartService, OrderService orderService)
        {
            _cartService = cartService;
            _orderService = orderService;
        }

        public async Task<IActionResult> Checkout()
        {
            Console.WriteLine("Checkout called");
            int? userId = User.Identity.IsAuthenticated
                ? int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
                : null;

            if (!userId.HasValue)
            {
                var cart = await _cartService.GetCart(null);
                if (cart.CartItems.Any())
                {
                    var cartItems = cart.CartItems.Select(ci => new CartItem
                    {
                        BouquetId = ci.BouquetId,
                        FlowerId = ci.FlowerId,
                        ToyId = ci.ToyId,
                        Quantity = ci.Quantity
                    }).ToList();
                    HttpContext.Session.SetString("CartItems", JsonSerializer.Serialize(cartItems));
                    await HttpContext.Session.CommitAsync();
                    Console.WriteLine("Session saved for non-authenticated user before redirect to login");
                }
                return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action("Checkout", "Cart") });
            }

            var userCart = await _cartService.GetCart(userId.Value);
            Console.WriteLine($"Checkout: userId={userId}, CartItemsCount={userCart.CartItems.Count}");
            return View(userCart);
        }

        [HttpGet]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> GetCartSummary()
        {
            Console.WriteLine("GetCartSummary called");
            int? userId = User.Identity.IsAuthenticated
                ? int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
                : null;

            var cart = await _cartService.GetCart(userId);
            Console.WriteLine($"GetCartSummary: userId={userId}, CartItemsCount={cart.CartItems.Count}");
            return PartialView("_CartPartial", cart);
        }

        [HttpPost]
        [Route("api/Cart/UpdateCartItem")]
        public async Task<IActionResult> UpdateCartItem([FromForm] int cartItemId, [FromForm] int quantity)
        {
            Console.WriteLine($"UpdateCartItem: cartItemId={cartItemId}, quantity={quantity}");
            if (quantity < 1)
            {
                return BadRequest(new { success = false, message = "Количество должно быть больше 0" });
            }

            int? userId = User.Identity.IsAuthenticated
                ? int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
                : null;

            try
            {
                await _cartService.UpdateCartItem(userId, cartItemId, quantity);
                return Ok(new { success = true, message = "Количество обновлено" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateCartItem: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Ошибка при обновлении: " + ex.Message });
            }
        }

        [HttpPost]
        [Route("api/Cart/RemoveCartItem")]
        public async Task<IActionResult> RemoveCartItem([FromForm] int cartItemId)
        {
            Console.WriteLine($"RemoveCartItem: cartItemId={cartItemId}");
            int? userId = User.Identity.IsAuthenticated
                ? int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
                : null;

            try
            {
                await _cartService.RemoveCartItem(userId, cartItemId);
                return Ok(new { success = true, message = "Товар удален из корзины" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RemoveCartItem: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Ошибка при удалении: " + ex.Message });
            }
        }

        [HttpPost]
        [Route("api/Cart/PlaceOrder")]
        public async Task<IActionResult> PlaceOrder([FromForm] string street, [FromForm] string city, [FromForm] string zip)
        {
            Console.WriteLine($"PlaceOrder: street={street}, city={city}, zip={zip}");
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { success = false, message = "Войдите в аккаунт" });
            }

            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var cart = await _cartService.GetCart(userId);
            var orderDto = new OrderDto
            {
                UserId = userId,
                OrderItems = cart.CartItems.Select(ci => new OrderItemDto
                {
                    BouquetId = ci.BouquetId,
                    FlowerId = ci.FlowerId,
                    ToyId = ci.ToyId,
                    Quantity = ci.Quantity,
                    Price = ci.Bouquet?.Price ?? ci.Flower?.Price ?? ci.Toy?.Price ?? 0
                }).ToList(),
                TotalAmount = cart.CartItems.Sum(ci => ci.Quantity * (ci.Bouquet?.Price ?? ci.Flower?.Price ?? ci.Toy?.Price ?? 0)),
                Street = street,
                City = city,
                Zip = zip
            };

            try
            {
                await _orderService.CreateOrder(orderDto);
                return Ok(new { success = true, message = "Заказ успешно оформлен" });
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"InvalidOperationException in PlaceOrder: {ex.Message}");
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PlaceOrder: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Ошибка при оформлении заказа: " + ex.Message });
            }
        }
    }
}