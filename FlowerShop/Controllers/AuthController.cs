using Microsoft.AspNetCore.Mvc;
using FlowerShop.Models;
using FlowerShop.Services;
using System.Threading.Tasks;

namespace FlowerShop.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService _authService;
        private readonly CartService _cartService;

        public AuthController(AuthService authService, CartService cartService)
        {
            _authService = authService;
            _cartService = cartService;
        }

        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto dto, string returnUrl = null)
        {
            var user = _authService.Login(dto);
            if (user == null)
            {
                ModelState.AddModelError("", "Неверное имя пользователя или пароль");
                ViewData["ReturnUrl"] = returnUrl;
                return View(dto);
            }

            await _cartService.MergeSessionCartToUserCart(user.Id);
            var token = _authService.GenerateJwtToken(user);
            HttpContext.Session.SetString("JWToken", token);

            return !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)
                ? Redirect(returnUrl)
                : RedirectToAction("Index", "Home");
        }

        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto dto, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ReturnUrl"] = returnUrl;
                return View(dto);
            }

            var user = _authService.Register(dto);
            if (user == null)
            {
                ModelState.AddModelError("", "Пользователь с таким именем уже существует");
                ViewData["ReturnUrl"] = returnUrl;
                return View(dto);
            }

            await _cartService.MergeSessionCartToUserCart(user.Id);
            var token = _authService.GenerateJwtToken(user);
            HttpContext.Session.SetString("JWToken", token);

            return !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)
                ? Redirect(returnUrl)
                : RedirectToAction("Checkout", "Cart");
        }
        [HttpPost]
        [Route("/Auth/Logout")]
        public IActionResult Logout()
        {
            try
            {
                HttpContext.Session.Remove("JWToken");
                HttpContext.Session.Clear();
                return Content("Выход выполнен успешно", "text/plain");
            }
            catch (Exception ex)
            {
                return Content($"Ошибка при выходе: {ex.Message}", "text/plain");
            }
        }
    }
}