using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlowerShop.Models;
using FlowerShop.Services;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace FlowerShop.Controllers
{
    public class FlowersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly CartService _cartService;
        private readonly FavoriteService _favoriteService;

        public FlowersController(ApplicationDbContext context, CartService cartService, FavoriteService favoriteService)
        {
            _context = context;
            _cartService = cartService;
            _favoriteService = favoriteService;
        }

        public async Task<IActionResult> Index()
        {
            var flowers = await _context.Flowers.ToListAsync();
            return View(flowers);
        }

        public async Task<IActionResult> Details(int id)
        {
            var flower = await _context.Flowers.FindAsync(id);
            if (flower == null)
            {
                return View("Error", new ErrorViewModel { Message = "Цветок не найден" });
            }

            var recommendations = await _context.Flowers
                .Where(f => f.Id != id)
                .Take(3)
                .ToListAsync();

            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.FlowerId == id)
                .ToListAsync();

            var viewModel = new FlowerDetailsViewModel
            {
                Flower = flower,
                Recommendations = recommendations,
                Reviews = reviews
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        [Route("/Flowers/AddReview")]
        public async Task<IActionResult> AddReview([FromForm] int id, [FromForm] string comment, [FromForm] int rating)
        {
            if (rating < 1 || rating > 5)
            {
                return Json(new { success = false, message = "Рейтинг должен быть от 1 до 5" });
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var flower = await _context.Flowers.FindAsync(id);
            if (flower == null)
            {
                return Json(new { success = false, message = "Цветок не найден" });
            }

            var review = new Review
            {
                UserId = userId,
                FlowerId = id,
                Comment = comment,
                Rating = rating,
                CreatedAt = DateTime.Now
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Отзыв добавлен",
                review = new
                {
                    id = review.Id,
                    username = User.Identity.Name,
                    rating = review.Rating,
                    comment = review.Comment,
                    createdAt = review.CreatedAt.ToString("dd.MM.yyyy"),
                    canDelete = true
                }
            });
        }

        [HttpPost]
        [Authorize]
        [Route("/Flowers/DeleteReview")]
        public async Task<IActionResult> DeleteReview([FromForm] int reviewId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var isAdmin = User.IsInRole("Admin");

            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null)
            {
                return Json(new { success = false, message = "Отзыв не найден" });
            }

            if (review.UserId != userId && !isAdmin)
            {
                return Json(new { success = false, message = "Нет прав для удаления отзыва" });
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Отзыв удалён" });
        }

        [HttpPost]
        [Route("/Flowers/AddToCart")]
        public async Task<IActionResult> AddToCart([FromForm] int id, [FromForm] int? Quantity = null)
        {
            int? userId = User.Identity.IsAuthenticated
                ? int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
                : null;

            var flower = await _context.Flowers.FindAsync(id);
            if (flower == null)
            {
                return Json(new { success = false, message = "Цветок не найден" });
            }

            int quantityToAdd = Quantity ?? 1;
            if (quantityToAdd <= 0)
            {
                return Json(new { success = false, message = "Количество должно быть больше 0" });
            }
            if (quantityToAdd > flower.Stock)
            {
                return Json(new { success = false, message = $"Недостаточно товара. Доступно: {flower.Stock}" });
            }

            await _cartService.AddToCart(userId, new CartItemDto { FlowerId = id, Quantity = quantityToAdd });
            return Json(new { success = true, message = "Цветок добавлен в корзину" });
        }

        [HttpPost]
        [Authorize]
        [Route("/Flowers/AddToFavorite")]
        public async Task<IActionResult> AddToFavorite([FromForm] int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var favorite = await _context.Favorites.FirstOrDefaultAsync(f => f.UserId == userId && f.FlowerId == id);

            if (favorite != null)
            {
                _context.Favorites.Remove(favorite);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Цветок удален из избранного" });
            }

            await _favoriteService.AddToFavorite(userId, flowerId: id);
            return Json(new { success = true, message = "Цветок добавлен в избранное" });
        }

        [HttpGet]
        [Authorize]
        [Route("/api/Flowers/IsFavorite")]
        public async Task<IActionResult> IsFavorite([FromQuery] int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var isFavorite = await _favoriteService.IsFavorite(userId, flowerId: id);
            return Json(new { isFavorite });
        }
    }
}