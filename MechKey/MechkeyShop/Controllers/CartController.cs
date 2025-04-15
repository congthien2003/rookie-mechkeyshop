using Application.Interfaces.IServices;
using MechkeyShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MechkeyShop.Controllers
{
    public class CartController : Controller
    {
        private readonly IApplicaionUserService applicaionUserService;
        public CartController(IApplicaionUserService applicaionUserService)
        {
            this.applicaionUserService = applicaionUserService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> Checkout()
        {
            try
            {
                var user = HttpContext.User;
                var id = user.FindFirst("Id")?.Value;

                var infoUser = await applicaionUserService.GetByIdAsync(Guid.Parse(id));

                var model = new CheckoutViewModel
                {
                    User = new UserModel
                    {
                        Name = infoUser.Data.Name,
                        Email = infoUser.Data.Email,
                        Phone = infoUser.Data.Phones,
                        Address = infoUser.Data.Address
                    },
                    CartItems = new List<CartItemModel>
            {
                new CartItemModel { ProductName = "Keycap Set", Quantity = 2, Price = 350000 },
                new CartItemModel { ProductName = "Switch Gateron", Quantity = 1, Price = 250000 }
            }
                };

                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }

    }
}
