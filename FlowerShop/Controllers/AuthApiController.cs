using Microsoft.AspNetCore.Mvc;
using FlowerShop.Models;
using FlowerShop.Services;

namespace FlowerShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthApiController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly CartService _cartService;

        public AuthApiController(AuthService authService, CartService cartService)
        {
            _authService = authService;
            _cartService = cartService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var user = _authService.Register(dto);
            await _cartService.MergeSessionCartToUserCart(user.Id);
            return Ok(new { Message = "User registered successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = _authService.Login(dto);
            if (user == null) return Unauthorized();
            await _cartService.MergeSessionCartToUserCart(user.Id);
            var token = _authService.GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            try
            {
                return Content("Выход выполнен успешно", "text/plain");
            }
            catch (Exception ex)
            {
                return Content($"Ошибка при выходе: {ex.Message}", "text/plain");
            }
        }
    }
}