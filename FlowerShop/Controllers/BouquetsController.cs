using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlowerShop.Models;
using FlowerShop.Services;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace FlowerShop.Controllers
{
    public class BouquetsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly CartService _cartService;
        private readonly FavoriteService _favoriteService;

        public BouquetsController(ApplicationDbContext context, CartService cartService, FavoriteService favoriteService)
        {
            _context = context;
            _cartService = cartService;
            _favoriteService = favoriteService;
        }

        public async Task<IActionResult> Index()
        {
            var bouquets = await _context.Bouquets.ToListAsync();
            return View(bouquets);
        }

        public async Task<IActionResult> Details(int id)
        {
            var bouquet = await _context.Bouquets.FindAsync(id);
            if (bouquet == null)
            {
                return View("Error", new ErrorViewModel { Message = "Букет не найден" });
            }

            var recommendations = await _context.Bouquets
                .Where(b => b.Id != id)
                .Take(3)
                .ToListAsync();

            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.BouquetId == id)
                .ToListAsync();

            var viewModel = new BouquetDetailsViewModel
            {
                Bouquet = bouquet,
                Recommendations = recommendations,
                Reviews = reviews
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        [Route("/Bouquets/AddReview")]
        public async Task<IActionResult> AddReview([FromForm] int id, [FromForm] string comment, [FromForm] int rating)
        {
            if (rating < 1 || rating > 5)
            {
                return Json(new { success = false, message = "Рейтинг должен быть от 1 до 5" });
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var bouquet = await _context.Bouquets.FindAsync(id);
            if (bouquet == null)
            {
                return Json(new { success = false, message = "Букет не найден" });
            }

            var review = new Review
            {
                UserId = userId,
                BouquetId = id,
                Comment = comment,
                Rating = rating,
                CreatedAt = DateTime.Now
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            // Возвращаем данные для динамического добавления отзыва
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
                    canDelete = true // Пользователь только что добавил отзыв
                }
            });
        }

        [HttpPost]
        [Authorize]
        [Route("/Bouquets/DeleteReview")]
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

        // Остальные методы (AddToCart, AddToFavorite, IsFavorite) без изменений
        [HttpPost]
        [Route("/Bouquets/AddToCart")]
        public async Task<IActionResult> AddToCart([FromForm] int id, [FromForm] int? Quantity = null)
        {
            int? userId = User.Identity.IsAuthenticated
                ? int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
                : null;

            var bouquet = await _context.Bouquets.FindAsync(id);
            if (bouquet == null)
            {
                return Json(new { success = false, message = "Букет не найден" });
            }

            int quantityToAdd = Quantity ?? 1;
            if (quantityToAdd <= 0)
            {
                return Json(new { success = false, message = "Количество должно быть больше 0" });
            }
            if (quantityToAdd > bouquet.Stock)
            {
                return Json(new { success = false, message = $"Недостаточно товара. Доступно: {bouquet.Stock}" });
            }

            await _cartService.AddToCart(userId, new CartItemDto { BouquetId = id, Quantity = quantityToAdd });
            return Json(new { success = true, message = "Букет добавлен в корзину" });
        }

        [HttpPost]
        [Authorize]
        [Route("/Bouquets/AddToFavorite")]
        public async Task<IActionResult> AddToFavorite([FromForm] int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var favorite = await _context.Favorites.FirstOrDefaultAsync(f => f.UserId == userId && f.BouquetId == id);

            if (favorite != null)
            {
                _context.Favorites.Remove(favorite);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Букет удален из избранного" });
            }

            await _favoriteService.AddToFavorite(userId, bouquetId: id);
            return Json(new { success = true, message = "Букет добавлен в избранное" });
        }

        [HttpGet]
        [Authorize]
        [Route("/api/Bouquets/IsFavorite")]
        public async Task<IActionResult> IsFavorite([FromQuery] int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var isFavorite = await _favoriteService.IsFavorite(userId, bouquetId: id);
            return Json(new { isFavorite });
        }
    }
}