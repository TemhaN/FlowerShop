using FlowerShop.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlowerShop.Services
{
    public class FavoriteService
    {
        private readonly ApplicationDbContext _context;

        public FavoriteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddToFavorite(int userId, int? bouquetId = null, int? flowerId = null, int? toyId = null)
        {
            var existing = await _context.Favorites.FirstOrDefaultAsync(f =>
                f.UserId == userId &&
                f.BouquetId == bouquetId &&
                f.FlowerId == flowerId &&
                f.ToyId == toyId);

            if (existing == null)
            {
                var favorite = new Favorite
                {
                    UserId = userId,
                    BouquetId = bouquetId,
                    FlowerId = flowerId,
                    ToyId = toyId,
                };
                _context.Favorites.Add(favorite);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsFavorite(int userId, int? bouquetId = null, int? flowerId = null, int? toyId = null)
        {
            return await _context.Favorites.AnyAsync(f =>
                f.UserId == userId &&
                f.BouquetId == bouquetId &&
                f.FlowerId == flowerId &&
                f.ToyId == toyId);
        }

        public async Task RemoveFromFavorite(int userId, int? bouquetId = null, int? flowerId = null, int? toyId = null)
        {
            var favorite = await _context.Favorites.FirstOrDefaultAsync(f =>
                f.UserId == userId &&
                f.BouquetId == bouquetId &&
                f.FlowerId == flowerId &&
                f.ToyId == toyId);

            if (favorite != null)
            {
                _context.Favorites.Remove(favorite);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    // Логируем, но не прерываем выполнение, так как запись уже удалена
                    Console.WriteLine($"Concurrency error in RemoveFromFavorite: userId={userId}, bouquetId={bouquetId}, flowerId={flowerId}, toyId={toyId}, Error: {ex.Message}");
                }
            }
            // Если favorite == null, ничего не делаем, так как записи уже нет
            else
            {
                Console.WriteLine($"Favorite not found: userId={userId}, bouquetId={bouquetId}, flowerId={flowerId}, toyId={toyId}");
            }
        }
        public async Task<List<FavoriteDto>> GetUserFavorites(int userId)
        {
            return await _context.Favorites
                .Include(f => f.Bouquet)
                .Include(f => f.Flower)
                .Include(f => f.Toy)
                .Where(f => f.UserId == userId)
                .Select(f => new FavoriteDto
                {
                    Id = f.Id,
                    UserId = f.UserId,
                    BouquetId = f.BouquetId,
                    BouquetName = f.Bouquet != null ? f.Bouquet.Name : null,
                    BouquetImagePath = f.Bouquet != null ? f.Bouquet.ImagePath : null,
                    BouquetPrice = f.Bouquet != null ? f.Bouquet.Price : null,
                    FlowerId = f.FlowerId,
                    FlowerName = f.Flower != null ? f.Flower.Name : null,
                    FlowerImagePath = f.Flower != null ? f.Flower.ImagePath : null,
                    FlowerPrice = f.Flower != null ? f.Flower.Price : null,
                    ToyId = f.ToyId,
                    ToyName = f.Toy != null ? f.Toy.Name : null,
                    ToyImagePath = f.Toy != null ? f.Toy.ImagePath : null,
                    ToyPrice = f.Toy != null ? f.Toy.Price : null
                })
                .ToListAsync();
        }
    }
}