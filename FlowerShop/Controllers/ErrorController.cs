using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult Index(int statusCode)
        {
            if (statusCode == 401)
            {
                return View("Error401");
            }
            return View("Error");
        }
    }
}