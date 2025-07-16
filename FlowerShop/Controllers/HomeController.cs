using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlowerShop.Models;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace FlowerShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromBody] SearchViewModel search)
        {
            // Логируем входные данные
            Console.WriteLine($"Получены данные: {System.Text.Json.JsonSerializer.Serialize(search)}");

            // Проверяем, что search не null
            search ??= new SearchViewModel();

            // 1. Загрузка данных
            var bouquets = await _context.Bouquets.ToListAsync();
            var flowers = await _context.Flowers.ToListAsync();
            var toys = await _context.Toys.ToListAsync();

            Console.WriteLine($"Букеты: {bouquets.Count}, Цветы: {flowers.Count}, Игрушки: {toys.Count}");

            // 2. Преобразование в ProductViewModel
            var allProducts = new List<ProductViewModel>();

            if (search.IncludeBouquets)
            {
                allProducts.AddRange(bouquets.Select(b => new ProductViewModel
                {
                    Id = b.Id,
                    Type = "Bouquet",
                    Name = b.Name,
                    Description = b.Description,
                    Price = b.Price,
                    Stock = b.Stock,
                    ImagePath = b.ImagePath,
                    DetailsUrl = $"/Bouquets/Details/{b.Id}",
                    Controller = "Bouquets"
                }));
            }

            if (search.IncludeFlowers)
            {
                allProducts.AddRange(flowers.Select(f => new ProductViewModel
                {
                    Id = f.Id,
                    Type = "Flower",
                    Name = f.Name,
                    Description = f.Description,
                    Price = f.Price,
                    Stock = f.Stock,
                    ImagePath = f.ImagePath,
                    DetailsUrl = $"/Flowers/Details/{f.Id}",
                    Controller = "Flowers"
                }));
            }

            if (search.IncludeToys)
            {
                allProducts.AddRange(toys.Select(t => new ProductViewModel
                {
                    Id = t.Id,
                    Type = "Toy",
                    Name = t.Name,
                    Description = t.Description,
                    Price = t.Price,
                    Stock = t.Stock,
                    ImagePath = t.ImagePath,
                    DetailsUrl = $"/Toys/Details/{t.Id}",
                    Controller = "Toys"
                }));
            }

            Console.WriteLine($"Всего продуктов после включения: {allProducts.Count}");

            // 3. Фильтрация
            if (search.ParsedMinPrice.HasValue)
            {
                allProducts = allProducts.Where(p => p.Price >= search.ParsedMinPrice.Value).ToList();
                Console.WriteLine($"После MinPrice {search.ParsedMinPrice.Value}: {allProducts.Count}");
            }

            if (search.ParsedMaxPrice.HasValue)
            {
                allProducts = allProducts.Where(p => p.Price <= search.ParsedMaxPrice.Value).ToList();
                Console.WriteLine($"После MaxPrice {search.ParsedMaxPrice.Value}: {allProducts.Count}");
            }

            if (!string.IsNullOrEmpty(search.Query))
            {
                var queryLower = search.Query.ToLower();
                allProducts = allProducts.Where(p => p.Name.ToLower().Contains(queryLower) ||
                                                    (p.Description?.ToLower().Contains(queryLower) ?? false)).ToList();
                Console.WriteLine($"После Query '{search.Query}': {allProducts.Count}");
            }

            // 4. Сортировка
            allProducts = search.SortBy switch
            {
                "PriceAsc" => allProducts.OrderBy(p => p.Price).ToList(),
                "PriceDesc" => allProducts.OrderByDescending(p => p.Price).ToList(),
                "NameAsc" => allProducts.OrderBy(p => p.Name).ToList(),
                "NameDesc" => allProducts.OrderByDescending(p => p.Name).ToList(),
                _ => allProducts.OrderBy(p => p.Name).ToList()
            };

            Console.WriteLine($"После сортировки ({search.SortBy}): {allProducts.Count}");

            // 5. Пагинация
            int totalCount = allProducts.Count;
            int skip = (search.Page - 1) * search.Size;
            var products = allProducts.Skip(skip).Take(search.Size).ToList();
            bool hasMore = skip + products.Count < totalCount;

            Console.WriteLine($"Продукты на странице {search.Page}: {products.Count}, HasMore: {hasMore}");

            // 6. Подготовка модели
            var model = new HomeViewModel
            {
                Products = products,
                Search = search,
                HasMore = hasMore
            };

            // 7. Возврат результата
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ProductsPartial", model);
            }

            return View(model);
        }

        public async Task<IActionResult> Index()
        {
            // Создаём модель с настройками по умолчанию
            var search = new SearchViewModel
            {
                IncludeBouquets = true,
                IncludeFlowers = true,
                IncludeToys = true,
                SortBy = "NameAsc",
                Page = 1,
                Size = 12
            };

            // Загружаем данные
            var bouquets = await _context.Bouquets.ToListAsync();
            var flowers = await _context.Flowers.ToListAsync();
            var toys = await _context.Toys.ToListAsync();

            Console.WriteLine($"GET: Букеты: {bouquets.Count}, Цветы: {flowers.Count}, Игрушки: {toys.Count}");

            // Преобразуем в ProductViewModel
            var allProducts = new List<ProductViewModel>();

            if (search.IncludeBouquets)
            {
                allProducts.AddRange(bouquets.Select(b => new ProductViewModel
                {
                    Id = b.Id,
                    Type = "Bouquet",
                    Name = b.Name,
                    Description = b.Description,
                    Price = b.Price,
                    Stock = b.Stock,
                    ImagePath = b.ImagePath,
                    DetailsUrl = $"/Bouquets/Details/{b.Id}",
                    Controller = "Bouquets"
                }));
            }

            if (search.IncludeFlowers)
            {
                allProducts.AddRange(flowers.Select(f => new ProductViewModel
                {
                    Id = f.Id,
                    Type = "Flower",
                    Name = f.Name,
                    Description = f.Description,
                    Price = f.Price,
                    Stock = f.Stock,
                    ImagePath = f.ImagePath,
                    DetailsUrl = $"/Flowers/Details/{f.Id}",
                    Controller = "Flowers"
                }));
            }

            if (search.IncludeToys)
            {
                allProducts.AddRange(toys.Select(t => new ProductViewModel
                {
                    Id = t.Id,
                    Type = "Toy",
                    Name = t.Name,
                    Description = t.Description,
                    Price = t.Price,
                    Stock = t.Stock,
                    ImagePath = t.ImagePath,
                    DetailsUrl = $"/Toys/Details/{t.Id}",
                    Controller = "Toys"
                }));
            }

            // Фильтрация
            if (!string.IsNullOrEmpty(search.Query))
            {
                var queryLower = search.Query.ToLower();
                allProducts = allProducts.Where(p => p.Name.ToLower().Contains(queryLower) ||
                                                    (p.Description?.ToLower().Contains(queryLower) ?? false)).ToList();
            }
            if (search.ParsedMinPrice.HasValue)
                allProducts = allProducts.Where(p => p.Price >= search.ParsedMinPrice.Value).ToList();
            if (search.ParsedMaxPrice.HasValue)
                allProducts = allProducts.Where(p => p.Price <= search.ParsedMaxPrice.Value).ToList();

            // Сортировка
            allProducts = search.SortBy switch
            {
                "PriceAsc" => allProducts.OrderBy(p => p.Price).ToList(),
                "PriceDesc" => allProducts.OrderByDescending(p => p.Price).ToList(),
                "NameAsc" => allProducts.OrderBy(p => p.Name).ToList(),
                "NameDesc" => allProducts.OrderByDescending(p => p.Name).ToList(),
                _ => allProducts.OrderBy(p => p.Name).ToList()
            };

            // Перемешивание с чередованием типов
            var mixedProducts = MixProducts(allProducts);

            // Пагинация
            int totalCount = mixedProducts.Count;
            int skip = (search.Page - 1) * search.Size;
            var products = mixedProducts.Skip(skip).Take(search.Size).ToList();
            bool hasMore = skip + products.Count < totalCount;

            Console.WriteLine($"GET: Продукты на странице {search.Page}: {products.Count}, HasMore: {hasMore}");

            // Модель для представления
            var model = new HomeViewModel
            {
                Products = products,
                Search = search,
                HasMore = hasMore
            };

            return View(model);
        }

        // Метод для "контролируемого" перемешивания товаров
        private List<ProductViewModel> MixProducts(List<ProductViewModel> products)
        {
            var bouquets = products.Where(p => p.Type == "Bouquet").ToList();
            var flowers = products.Where(p => p.Type == "Flower").ToList();
            var toys = products.Where(p => p.Type == "Toy").ToList();

            var mixed = new List<ProductViewModel>();
            var random = new Random();

            // Собираем товары, чередуя типы, пока есть элементы
            while (bouquets.Any() || flowers.Any() || toys.Any())
            {
                var availableTypes = new List<string>();
                if (bouquets.Any()) availableTypes.Add("Bouquet");
                if (flowers.Any()) availableTypes.Add("Flower");
                if (toys.Any()) availableTypes.Add("Toy");

                if (!availableTypes.Any()) break;

                // Выбираем случайный тип из доступных
                var type = availableTypes[random.Next(availableTypes.Count)];

                // Выбираем случайный продукт указанного типа
                ProductViewModel product = null;
                if (type == "Bouquet" && bouquets.Any())
                {
                    product = bouquets[random.Next(bouquets.Count)];
                    bouquets.Remove(product);
                }
                else if (type == "Flower" && flowers.Any())
                {
                    product = flowers[random.Next(flowers.Count)];
                    flowers.Remove(product);
                }
                else if (type == "Toy" && toys.Any())
                {
                    product = toys[random.Next(toys.Count)];
                    toys.Remove(product);
                }

                if (product != null)
                {
                    mixed.Add(product);
                }
            }

            return mixed;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}