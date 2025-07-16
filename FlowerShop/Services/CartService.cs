using Microsoft.EntityFrameworkCore;
using FlowerShop.Models;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Services
{
    public class CartService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Cart> GetCart(int? userId)
        {
            if (userId.HasValue)
            {
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Bouquet)
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Flower)
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Toy)
                    .FirstOrDefaultAsync(c => c.UserId == userId.Value);

                if (cart == null)
                {
                    cart = new Cart { UserId = userId.Value, CartItems = new List<CartItem>() };
                    _context.Carts.Add(cart);
                    await _context.SaveChangesAsync();
                }

                return cart;
            }
            else
            {
                var session = _httpContextAccessor.HttpContext.Session;
                var cartItemsJson = session.GetString("CartItems");
                var cartItems = string.IsNullOrEmpty(cartItemsJson)
                    ? new List<CartItem>()
                    : JsonSerializer.Deserialize<List<CartItem>>(cartItemsJson);

                foreach (var item in cartItems)
                {
                    if (item.BouquetId.HasValue)
                        item.Bouquet = await _context.Bouquets.FindAsync(item.BouquetId);
                    if (item.FlowerId.HasValue)
                        item.Flower = await _context.Flowers.FindAsync(item.FlowerId);
                    if (item.ToyId.HasValue)
                        item.Toy = await _context.Toys.FindAsync(item.ToyId);
                }

                return new Cart { CartItems = cartItems };
            }
        }

        public async Task AddToCart(int? userId, CartItemDto cartItemDto)
        {
            Console.WriteLine($"CartService.AddToCart called with userId={userId}, BouquetId={cartItemDto.BouquetId}, FlowerId={cartItemDto.FlowerId}, ToyId={cartItemDto.ToyId}, Quantity={cartItemDto.Quantity}");

            int stock = 0;
            string itemType = "";
            if (cartItemDto.BouquetId.HasValue)
            {
                var bouquet = await _context.Bouquets.FindAsync(cartItemDto.BouquetId);
                if (bouquet == null) throw new InvalidOperationException("Букет не найден");
                stock = bouquet.Stock;
                itemType = "Букет";
            }
            else if (cartItemDto.FlowerId.HasValue)
            {
                var flower = await _context.Flowers.FindAsync(cartItemDto.FlowerId);
                if (flower == null) throw new InvalidOperationException("Цветок не найден");
                stock = flower.Stock;
                itemType = "Цветок";
            }
            else if (cartItemDto.ToyId.HasValue)
            {
                var toy = await _context.Toys.FindAsync(cartItemDto.ToyId);
                if (toy == null) throw new InvalidOperationException("Игрушка не найдена");
                stock = toy.Stock;
                itemType = "Игрушка";
            }
            else
            {
                throw new InvalidOperationException("Не указан идентификатор товара");
            }

            if (cartItemDto.Quantity <= 0)
            {
                throw new InvalidOperationException("Количество должно быть больше 0");
            }

            if (cartItemDto.Quantity > stock)
            {
                throw new InvalidOperationException($"Недостаточно {itemType.ToLower()} в наличии. Доступно: {stock}");
            }

            if (userId.HasValue)
            {
                var cart = await GetCart(userId.Value);
                var existingItem = cart.CartItems.FirstOrDefault(ci =>
                    ci.BouquetId == cartItemDto.BouquetId &&
                    ci.FlowerId == cartItemDto.FlowerId &&
                    ci.ToyId == cartItemDto.ToyId);

                if (existingItem != null)
                {
                    if (existingItem.Quantity + cartItemDto.Quantity > stock)
                    {
                        throw new InvalidOperationException($"Нельзя добавить {cartItemDto.Quantity} {itemType.ToLower()}. Доступно: {stock - existingItem.Quantity}");
                    }
                    existingItem.Quantity += cartItemDto.Quantity;
                    Console.WriteLine($"Updated existing cart item: {itemType} Id={cartItemDto.ToyId ?? cartItemDto.FlowerId ?? cartItemDto.BouquetId}, New Quantity={existingItem.Quantity}");
                }
                else
                {
                    var cartItem = new CartItem
                    {
                        CartId = cart.Id,
                        BouquetId = cartItemDto.BouquetId,
                        FlowerId = cartItemDto.FlowerId,
                        ToyId = cartItemDto.ToyId,
                        Quantity = cartItemDto.Quantity
                    };
                    cart.CartItems.Add(cartItem);
                    Console.WriteLine($"Added new cart item: {itemType} Id={cartItemDto.ToyId ?? cartItemDto.FlowerId ?? cartItemDto.BouquetId}, Quantity={cartItemDto.Quantity}");
                }

                await _context.SaveChangesAsync();
            }
            else
            {
                var session = _httpContextAccessor.HttpContext.Session;
                var cartItemsJson = session.GetString("CartItems");
                var cartItems = string.IsNullOrEmpty(cartItemsJson)
                    ? new List<CartItem>()
                    : JsonSerializer.Deserialize<List<CartItem>>(cartItemsJson);

                var existingItem = cartItems.FirstOrDefault(ci =>
                    ci.BouquetId == cartItemDto.BouquetId &&
                    ci.FlowerId == cartItemDto.FlowerId &&
                    ci.ToyId == cartItemDto.ToyId);

                if (existingItem != null)
                {
                    if (existingItem.Quantity + cartItemDto.Quantity > stock)
                    {
                        throw new InvalidOperationException($"Нельзя добавить {cartItemDto.Quantity} {itemType.ToLower()}. Доступно: {stock - existingItem.Quantity}");
                    }
                    existingItem.Quantity += cartItemDto.Quantity;
                    Console.WriteLine($"Updated session cart item: {itemType} Id={cartItemDto.ToyId ?? cartItemDto.FlowerId ?? cartItemDto.BouquetId}, New Quantity={existingItem.Quantity}");
                }
                else
                {
                    cartItems.Add(new CartItem
                    {
                        BouquetId = cartItemDto.BouquetId,
                        FlowerId = cartItemDto.FlowerId,
                        ToyId = cartItemDto.ToyId,
                        Quantity = cartItemDto.Quantity
                    });
                    Console.WriteLine($"Added new session cart item: {itemType} Id={cartItemDto.ToyId ?? cartItemDto.FlowerId ?? cartItemDto.BouquetId}, Quantity={cartItemDto.Quantity}");
                }

                session.SetString("CartItems", JsonSerializer.Serialize(cartItems));
            }
        }
        public async Task UpdateCartItem(int? userId, int cartItemId, int quantity)
        {
            if (quantity <= 0)
            {
                await RemoveCartItem(userId, cartItemId);
                return;
            }

            if (userId.HasValue)
            {
                var cart = await GetCart(userId.Value);
                var item = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
                if (item != null)
                {
                    item.Quantity = quantity;
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                var session = _httpContextAccessor.HttpContext.Session;
                var cartItemsJson = session.GetString("CartItems");
                var cartItems = string.IsNullOrEmpty(cartItemsJson)
                    ? new List<CartItem>()
                    : JsonSerializer.Deserialize<List<CartItem>>(cartItemsJson);

                var item = cartItems.FirstOrDefault(ci =>
                    (ci.BouquetId.HasValue && ci.BouquetId == cartItemId) ||
                    (ci.FlowerId.HasValue && ci.FlowerId == cartItemId) ||
                    (ci.ToyId.HasValue && ci.ToyId == cartItemId));

                if (item != null)
                {
                    item.Quantity = quantity;
                    session.SetString("CartItems", JsonSerializer.Serialize(cartItems));
                }
            }
        }

        public async Task RemoveCartItem(int? userId, int cartItemId)
        {
            if (userId.HasValue)
            {
                var cart = await GetCart(userId.Value);
                var item = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
                if (item != null)
                {
                    _context.CartItems.Remove(item);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                var session = _httpContextAccessor.HttpContext.Session;
                var cartItemsJson = session.GetString("CartItems");
                var cartItems = string.IsNullOrEmpty(cartItemsJson)
                    ? new List<CartItem>()
                    : JsonSerializer.Deserialize<List<CartItem>>(cartItemsJson);
                
                var item = cartItems.FirstOrDefault(ci =>
                    (ci.BouquetId.HasValue && ci.BouquetId == cartItemId) ||
                    (ci.FlowerId.HasValue && ci.FlowerId == cartItemId) ||
                    (ci.ToyId.HasValue && ci.ToyId == cartItemId));

                if (item != null)
                {
                    cartItems.Remove(item);
                    session.SetString("CartItems", JsonSerializer.Serialize(cartItems));
                }
            }
        }

        public async Task ClearCart(int? userId)
        {
            if (userId.HasValue)
            {
                var cart = await GetCart(userId.Value);
                _context.CartItems.RemoveRange(cart.CartItems);
                await _context.SaveChangesAsync();
            }
            else
            {
                _httpContextAccessor.HttpContext.Session.Remove("CartItems");
            }
        }

        public async Task MergeSessionCartToUserCart(int userId)
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var cartItemsJson = session.GetString("CartItems");
            if (!string.IsNullOrEmpty(cartItemsJson))
            {
                var sessionCartItems = JsonSerializer.Deserialize<List<CartItem>>(cartItemsJson);
                foreach (var item in sessionCartItems)
                {
                    await AddToCart(userId, new CartItemDto
                    {
                        BouquetId = item.BouquetId,
                        FlowerId = item.FlowerId,
                        ToyId = item.ToyId,
                        Quantity = item.Quantity
                    });
                }
                session.Remove("CartItems");
            }
        }
    }
}