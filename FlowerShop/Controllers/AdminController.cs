using FlowerShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FlowerShop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminController> _logger;
        private readonly string _imageUploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images");

        public AdminController(ApplicationDbContext context, ILogger<AdminController> logger)
        {
            _logger = logger;
            _context = context;
            if (!Directory.Exists(_imageUploadPath))
            {
                Directory.CreateDirectory(_imageUploadPath);
            }
        }

        public async Task<IActionResult> Dashboard()
        {
            var model = new AdminDashboardViewModel
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalOrders = await _context.Orders.CountAsync(),
                TotalBouquets = await _context.Bouquets.CountAsync(),
                TotalFlowers = await _context.Flowers.CountAsync(),
                TotalToys = await _context.Toys.CountAsync(),
                TotalRevenue = await _context.Orders.SumAsync(o => o.TotalAmount),
                TotalReviews = await _context.Reviews.CountAsync()
            };
            return View(model);
        }

        // Управление отзывами
        public async Task<IActionResult> Reviews()
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Bouquet)
                .Include(r => r.Flower)
                .Include(r => r.Toy)
                .ToListAsync();
            return View(reviews);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReview([FromBody] DeleteDto dto)
        {
            _logger.LogInformation($"Attempting to delete review with ID: {dto.Id}");

            if (dto.Id <= 0)
            {
                _logger.LogWarning($"Invalid review ID: {dto.Id}");
                return Json(new { success = false, message = "Неверный ID отзыва" });
            }

            var review = await _context.Reviews.FindAsync(dto.Id);
            if (review == null)
            {
                _logger.LogWarning($"Review with ID {dto.Id} not found");
                return Json(new { success = false, message = "Отзыв не найден" });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation($"Review with ID {dto.Id} deleted successfully");
                return Json(new { success = true, message = "Отзыв удалён успешно" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Error deleting review with ID: {dto.Id}");
                return Json(new { success = false, message = $"Ошибка при удалении: {ex.Message}" });
            }
        }

        // Управление пользователями
        public async Task<IActionResult> Users()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDto userDto)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(u => u.Username == userDto.Username))
                {
                    return Json(new { success = false, message = "Пользователь с таким именем уже существует" });
                }
                var user = new User
                {
                    Username = userDto.Username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
                    Role = userDto.Role
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Пользователь создан успешно", data = new { id = user.Id, username = user.Username, role = user.Role } });
            }
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }

        [HttpPost]
        public async Task<IActionResult> EditUser([FromBody] UserEditDto userDto)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _context.Users.FindAsync(userDto.Id);
                if (existingUser == null) return Json(new { success = false, message = "Пользователь не найден" });
                existingUser.Username = userDto.Username;
                existingUser.Role = userDto.Role;
                if (!string.IsNullOrEmpty(userDto.Password))
                {
                    existingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
                }
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Пользователь обновлен успешно", data = new { id = existingUser.Id, username = existingUser.Username, role = existingUser.Role } });
            }
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser([FromBody] DeleteDto dto)
        {
            _logger.LogInformation($"Attempting to delete user with ID: {dto.Id}");

            if (dto.Id <= 0)
            {
                _logger.LogWarning($"Invalid user ID: {dto.Id}");
                return Json(new { success = false, message = "Неверный ID пользователя" });
            }

            var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            if (dto.Id == currentUserId)
            {
                _logger.LogWarning($"User {currentUserId} attempted to delete themselves");
                return Json(new { success = false, message = "Нельзя удалить самого себя" });
            }

            var user = await _context.Users.FindAsync(dto.Id);
            if (user == null)
            {
                var existingIds = await _context.Users.Select(u => u.Id).ToListAsync();
                _logger.LogWarning($"User with ID {dto.Id} not found. Existing user IDs: [{string.Join(", ", existingIds)}]");
                return Json(new { success = false, message = "Пользователь не найден" });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var carts = await _context.Carts.Where(c => c.UserId == dto.Id).ToListAsync();
                if (carts.Any())
                {
                    _context.Carts.RemoveRange(carts);
                    _logger.LogInformation($"Removed {carts.Count} carts for user ID: {dto.Id}");
                }

                var orders = await _context.Orders.Where(o => o.UserId == dto.Id).ToListAsync();
                if (orders.Any())
                {
                    _context.Orders.RemoveRange(orders);
                    _logger.LogInformation($"Removed {orders.Count} orders for user ID: {dto.Id}");
                }

                var favorites = await _context.Favorites.Where(f => f.UserId == dto.Id).ToListAsync();
                if (favorites.Any())
                {
                    _context.Favorites.RemoveRange(favorites);
                    _logger.LogInformation($"Removed {favorites.Count} favorites for user ID: {dto.Id}");
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation($"User with ID {dto.Id} deleted successfully");
                return Json(new { success = true, message = "Пользователь удален успешно" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Error deleting user with ID: {dto.Id}");
                return Json(new { success = false, message = $"Ошибка при удалении: {ex.Message}" });
            }
        }

        // Управление заказами
        public async Task<IActionResult> Orders()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ToListAsync();
            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteOrder([FromBody] DeleteDto dto)
        {
            _logger.LogInformation($"Attempting to delete order with ID: {dto.Id}");

            if (dto.Id <= 0)
            {
                _logger.LogWarning($"Invalid order ID: {dto.Id}");
                return Json(new { success = false, message = "Неверный ID заказа" });
            }

            var order = await _context.Orders.FindAsync(dto.Id);
            if (order == null)
            {
                _logger.LogWarning($"Order with ID {dto.Id} not found");
                return Json(new { success = false, message = "Заказ не найден" });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation($"Order with ID {dto.Id} deleted successfully");
                return Json(new { success = true, message = "Заказ удален успешно" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Error deleting order with ID: {dto.Id}");
                return Json(new { success = false, message = $"Ошибка при удалении: {ex.Message}" });
            }
        }

        // Управление букетами
        public async Task<IActionResult> Bouquets()
        {
            var bouquets = await _context.Bouquets.ToListAsync();
            return View(bouquets);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBouquet([FromForm] BouquetCreateDto bouquetDto)
        {
            if (ModelState.IsValid)
            {
                string imagePath = "/Images/placeholder.png";
                if (bouquetDto.ImageFile != null && bouquetDto.ImageFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(bouquetDto.ImageFile.FileName);
                    var filePath = Path.Combine(_imageUploadPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await bouquetDto.ImageFile.CopyToAsync(stream);
                    }
                    imagePath = $"/Images/{fileName}";
                }

                var bouquet = new Bouquet
                {
                    Name = bouquetDto.Name,
                    Description = bouquetDto.Description,
                    Price = bouquetDto.Price,
                    Stock = bouquetDto.Stock,
                    ImagePath = imagePath
                };

                _context.Bouquets.Add(bouquet);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Букет создан успешно", data = bouquet });
            }
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }

        [HttpPost]
        public async Task<IActionResult> EditBouquet([FromForm] BouquetEditDto bouquetDto)
        {
            if (ModelState.IsValid)
            {
                var existingBouquet = await _context.Bouquets.FindAsync(bouquetDto.Id);
                if (existingBouquet == null) return Json(new { success = false, message = "Букет не найден" });

                if (bouquetDto.ImageFile != null && bouquetDto.ImageFile.Length > 0)
                {
                    if (!string.IsNullOrEmpty(existingBouquet.ImagePath) && existingBouquet.ImagePath != "/Images/placeholder.png")
                    {
                        var oldFilePath = Path.Combine("wwwroot", existingBouquet.ImagePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(bouquetDto.ImageFile.FileName);
                    var filePath = Path.Combine(_imageUploadPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await bouquetDto.ImageFile.CopyToAsync(stream);
                    }
                    existingBouquet.ImagePath = $"/Images/{fileName}";
                }

                existingBouquet.Name = bouquetDto.Name;
                existingBouquet.Description = bouquetDto.Description;
                existingBouquet.Price = bouquetDto.Price;
                existingBouquet.Stock = bouquetDto.Stock;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Букет обновлен успешно", data = existingBouquet });
            }
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBouquet([FromBody] DeleteDto dto)
        {
            _logger.LogInformation($"Attempting to delete bouquet with ID: {dto.Id}");

            if (dto.Id <= 0)
            {
                _logger.LogWarning($"Invalid bouquet ID: {dto.Id}");
                return Json(new { success = false, message = "Неверный ID букета" });
            }

            var bouquet = await _context.Bouquets.FindAsync(dto.Id);
            if (bouquet == null)
            {
                _logger.LogWarning($"Bouquet with ID {dto.Id} not found");
                return Json(new { success = false, message = "Букет не найден" });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (!string.IsNullOrEmpty(bouquet.ImagePath) && bouquet.ImagePath != "/Images/placeholder.png")
                {
                    var filePath = Path.Combine("wwwroot", bouquet.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Bouquets.Remove(bouquet);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation($"Bouquet with ID {dto.Id} deleted successfully");
                return Json(new { success = true, message = "Букет удалён успешно" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Error deleting bouquet with ID: {dto.Id}");
                return Json(new { success = false, message = $"Ошибка при удалении: {ex.Message}" });
            }
        }

        // Управление цветами
        public async Task<IActionResult> Flowers()
        {
            var flowers = await _context.Flowers.ToListAsync();
            return View(flowers);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFlower([FromForm] FlowerCreateDto flowerDto)
        {
            if (ModelState.IsValid)
            {
                string imagePath = "/Images/placeholder.png";
                if (flowerDto.ImageFile != null && flowerDto.ImageFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(flowerDto.ImageFile.FileName);
                    var filePath = Path.Combine(_imageUploadPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await flowerDto.ImageFile.CopyToAsync(stream);
                    }
                    imagePath = $"/Images/{fileName}";
                }

                var flower = new Flower
                {
                    Name = flowerDto.Name,
                    Description = flowerDto.Description,
                    Price = flowerDto.Price,
                    Stock = flowerDto.Stock,
                    ImagePath = imagePath
                };

                _context.Flowers.Add(flower);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Цветок создан успешно", data = flower });
            }
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }

        [HttpPost]
        public async Task<IActionResult> EditFlower([FromForm] FlowerEditDto flowerDto)
        {
            if (ModelState.IsValid)
            {
                var existingFlower = await _context.Flowers.FindAsync(flowerDto.Id);
                if (existingFlower == null) return Json(new { success = false, message = "Цветок не найден" });

                if (flowerDto.ImageFile != null && flowerDto.ImageFile.Length > 0)
                {
                    if (!string.IsNullOrEmpty(existingFlower.ImagePath) && existingFlower.ImagePath != "/Images/placeholder.png")
                    {
                        var oldFilePath = Path.Combine("wwwroot", existingFlower.ImagePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(flowerDto.ImageFile.FileName);
                    var filePath = Path.Combine(_imageUploadPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await flowerDto.ImageFile.CopyToAsync(stream);
                    }
                    existingFlower.ImagePath = $"/Images/{fileName}";
                }

                existingFlower.Name = flowerDto.Name;
                existingFlower.Description = flowerDto.Description;
                existingFlower.Price = flowerDto.Price;
                existingFlower.Stock = flowerDto.Stock;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Цветок обновлён успешно", data = existingFlower });
            }
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFlower([FromBody] DeleteDto dto)
        {
            _logger.LogInformation($"Attempting to delete flower with ID: {dto.Id}");

            if (dto.Id <= 0)
            {
                _logger.LogWarning($"Invalid flower ID: {dto.Id}");
                return Json(new { success = false, message = "Неверный ID цветка" });
            }

            var flower = await _context.Flowers.FindAsync(dto.Id);
            if (flower == null)
            {
                _logger.LogWarning($"Flower with ID {dto.Id} not found");
                return Json(new { success = false, message = "Цветок не найден" });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (!string.IsNullOrEmpty(flower.ImagePath) && flower.ImagePath != "/Images/placeholder.png")
                {
                    var filePath = Path.Combine("wwwroot", flower.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Flowers.Remove(flower);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation($"Flower with ID {dto.Id} deleted successfully");
                return Json(new { success = true, message = "Цветок успешно удалён" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Error deleting flower with ID: {dto.Id}");
                return Json(new { success = false, message = $"Ошибка при удалении: {ex.Message}" });
            }
        }

        // Управление игрушками
        public async Task<IActionResult> Toys()
        {
            var toys = await _context.Toys.ToListAsync();
            return View(toys);
        }

        [HttpPost]
        public async Task<IActionResult> CreateToy([FromForm] ToyCreateDto toyDto)
        {
            if (ModelState.IsValid)
            {
                string imagePath = "/Images/placeholder.png";
                if (toyDto.ImageFile != null && toyDto.ImageFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(toyDto.ImageFile.FileName);
                    var filePath = Path.Combine(_imageUploadPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await toyDto.ImageFile.CopyToAsync(stream);
                    }
                    imagePath = $"/Images/{fileName}";
                }

                var toy = new Toy
                {
                    Name = toyDto.Name,
                    Description = toyDto.Description,
                    Price = toyDto.Price,
                    Stock = toyDto.Stock,
                    ImagePath = imagePath
                };

                _context.Toys.Add(toy);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Игрушка успешно создана", data = toy });
            }
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }

        [HttpPost]
        public async Task<IActionResult> EditToy([FromForm] ToyEditDto toyDto)
        {
            if (ModelState.IsValid)
            {
                var existingToy = await _context.Toys.FindAsync(toyDto.Id);
                if (existingToy == null) return Json(new { success = false, message = "Игрушка не найдена" });

                if (toyDto.ImageFile != null && toyDto.ImageFile.Length > 0)
                {
                    if (!string.IsNullOrEmpty(existingToy.ImagePath) && existingToy.ImagePath != "/Images/placeholder.png")
                    {
                        var oldFilePath = Path.Combine("wwwroot", existingToy.ImagePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(toyDto.ImageFile.FileName);
                    var filePath = Path.Combine(_imageUploadPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await toyDto.ImageFile.CopyToAsync(stream);
                    }
                    existingToy.ImagePath = $"/Images/{fileName}";
                }

                existingToy.Name = toyDto.Name;
                existingToy.Description = toyDto.Description;
                existingToy.Price = toyDto.Price;
                existingToy.Stock = toyDto.Stock;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Игрушка успешно обновлена", data = existingToy });
            }
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteToy([FromBody] DeleteDto dto)
        {
            _logger.LogInformation($"Attempting to delete toy with ID: {dto.Id}");

            if (dto.Id <= 0)
            {
                _logger.LogWarning($"Invalid toy ID: {dto.Id}");
                return Json(new { success = false, message = "Неверный ID игрушки" });
            }

            var toy = await _context.Toys.FindAsync(dto.Id);
            if (toy == null)
            {
                _logger.LogWarning($"Toy with ID {dto.Id} not found");
                return Json(new { success = false, message = "Игрушка не найдена" });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (!string.IsNullOrEmpty(toy.ImagePath) && toy.ImagePath != "/Images/placeholder.png")
                {
                    var filePath = Path.Combine("wwwroot", toy.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Toys.Remove(toy);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation($"Toy with ID {dto.Id} deleted successfully");
                return Json(new { success = true, message = "Игрушка успешно удалена" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Error deleting toy with ID: {dto.Id}");
                return Json(new { success = false, message = $"Ошибка при удалении: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Statistics()
        {
            try
            {
                var startDate = DateTime.UtcNow.Date.AddDays(-6);
                var ordersByDay = await _context.Orders
                    .Where(o => o.OrderDate >= startDate)
                    .GroupBy(o => o.OrderDate.Date)
                    .Select(g => new { Date = g.Key, Count = g.Count() })
                    .ToListAsync();

                var labels = new string[7];
                var values = new int[7];
                for (int i = 0; i < 7; i++)
                {
                    var date = startDate.AddDays(i);
                    labels[i] = date.ToString("dd.MM");
                    values[i] = ordersByDay.FirstOrDefault(x => x.Date == date)?.Count ?? 0;
                }

                var topItems = await _context.OrderItems
                    .GroupBy(oi => new { oi.BouquetId, oi.FlowerId, oi.ToyId })
                    .Select(g => new
                    {
                        Name = g.Key.BouquetId.HasValue
                            ? _context.Bouquets.FirstOrDefault(b => b.Id == g.Key.BouquetId).Name
                            : g.Key.FlowerId.HasValue
                                ? _context.Flowers.FirstOrDefault(f => f.Id == g.Key.FlowerId).Name
                                : _context.Toys.FirstOrDefault(t => t.Id == g.Key.ToyId).Name,
                        Count = g.Sum(oi => oi.Quantity)
                    })
                    .OrderByDescending(x => x.Count)
                    .Take(5)
                    .ToListAsync();

                var topLabels = topItems.Select(x => x.Name).ToArray();
                var topValues = topItems.Select(x => x.Count).ToArray();

                return Json(new
                {
                    success = true,
                    ordersByDay = new { labels, values },
                    topItems = new { labels = topLabels, values = topValues }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке статистики");
                return Json(new { success = false, message = "Ошибка при загрузке статистики: " + ex.Message });
            }
        }

        public class DeleteDto
        {
            public int Id { get; set; }
        }

        public class UserCreateDto
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Role { get; set; }
        }

        public class UserEditDto
        {
            public int Id { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string Role { get; set; }
        }

        public class BouquetCreateDto
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public int Stock { get; set; }
            public IFormFile ImageFile { get; set; }
        }

        public class BouquetEditDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public int Stock { get; set; }
            public IFormFile ImageFile { get; set; }
        }

        public class FlowerCreateDto
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public int Stock { get; set; }
            public IFormFile ImageFile { get; set; }
        }

        public class FlowerEditDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public int Stock { get; set; }
            public IFormFile ImageFile { get; set; }
        }

        public class ToyCreateDto
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public int Stock { get; set; }
            public IFormFile ImageFile { get; set; }
        }

        public class ToyEditDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public int Stock { get; set; }
            public IFormFile ImageFile { get; set; }
        }
    }
}