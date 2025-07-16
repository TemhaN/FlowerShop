using Microsoft.AspNetCore.Mvc;
using FlowerShop.Models;
using FlowerShop.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FlowerShop.Controllers
{
    public class FavoritesController : Controller
    {
        private readonly FavoriteService _favoriteService;

        public FavoritesController(FavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var favorites = await _favoriteService.GetUserFavorites(userId);
            return View(favorites);
        }

    }
}