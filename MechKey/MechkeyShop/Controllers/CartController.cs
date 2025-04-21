using Application.Interfaces.IServices;
using MechkeyShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.ViewModels;

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
        public async Task<IActionResult> Checkout()
        {
            var user = HttpContext.User;
            var id = user.FindFirst("Id")?.Value;

            var infoUser = await applicaionUserService.GetByIdAsync(Guid.Parse(id));

            var model = new CheckoutViewModel
            {
                User = new UserModel
                {
                    Id = infoUser.Data.Id,
                    Name = infoUser.Data.Name,
                    Email = infoUser.Data.Email,
                    Phone = infoUser.Data.Phones,
                    Address = infoUser.Data.Address
                }
            };

            return View(model);
        }


        [HttpPost]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> SubmitOrder(CreateOrderModel model)
        {
            var user = HttpContext.User;
            var id = user.FindFirst("Id")?.Value;
            var infoUser = await applicaionUserService.GetByIdAsync(Guid.Parse(id));

            model.UserId = Guid.Parse(id);

            return Ok();
        }
    }
}
