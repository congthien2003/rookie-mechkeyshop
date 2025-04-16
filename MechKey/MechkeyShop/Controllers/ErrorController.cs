using Microsoft.AspNetCore.Mvc;

namespace MechkeyShop.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error")]
        public IActionResult Error()
        {
            return View(); // Views/Error/Error.cshtml
        }

        [Route("Error/NotFound")]
        public IActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View("NotFound");
        }
    }
}
