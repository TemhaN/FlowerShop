using Microsoft.AspNetCore.Mvc;
using FlowerShop.Models;
using FlowerShop.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FlowerShop.Controllers
{
    public class UserController : Controller
    {
        private readonly UserService _userService;
        private readonly FavoriteService _favoriteService;
        private readonly OrderService _orderService;
        private readonly CartService _cartService;

        public UserController(UserService userService, FavoriteService favoriteService, OrderService orderService, CartService cartService)
        {
            _userService = userService;
            _favoriteService = favoriteService;
            _orderService = orderService;
            _cartService = cartService;
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = _userService.GetUserById(userId);
            var model = new ProfileViewModel
            {
                UserProfile = new UserProfileDto
                {
                    Id = user.Id,
                    Username = user.Username
                },
                Favorites = await _favoriteService.GetUserFavorites(userId),
                Orders = await _orderService.GetUserOrders(userId),
                Cart = await _cartService.GetCart(userId)
            };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UserProfileDto dto)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (!ModelState.IsValid)
            {
                var model = new ProfileViewModel
                {
                    UserProfile = dto,
                    Favorites = await _favoriteService.GetUserFavorites(currentUserId),
                    Orders = await _orderService.GetUserOrders(currentUserId),
                    Cart = await _cartService.GetCart(currentUserId)
                };
                return View("Profile", model);
            }

            dto.Id = currentUserId;
            _userService.UpdateUser(dto);
            return RedirectToAction("Profile");
        }
    }
}