using Application.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationUserController : ControllerBase
    {
        private readonly IApplicaionUserService applicaionUserService;

        public ApplicationUserController(IApplicaionUserService applicaionUserService)
        {
            this.applicaionUserService = applicaionUserService;
        }

        // Add new user
        //[HttpPost]
        //public async Task<Result<ApplicationUserModel>> Add(ApplicationUserModel model)
        //{
        //    if (model is null)
        //    {
        //        return Result<ApplicationUserModel>.Failure("Model is null", null);
        //    }

        //    return await applicaionUserService.AddAsync(model);
        //}
    }
}
