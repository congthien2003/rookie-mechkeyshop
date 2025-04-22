using Application.Comoon;
using Application.Interfaces.IServices;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;
using Shared.ViewModels;

namespace WebAPI.Controllers
{
    [ApiVersion(1)]
    [Route("api/v{v:apiVersion}/users")]
    [ApiController]
    public class ApplicationUserController : ControllerBase
    {
        private readonly IApplicaionUserService applicationUserService;

        public ApplicationUserController(IApplicaionUserService applicationUserService)
        {
            this.applicationUserService = applicationUserService;
        }


        [HttpGet("list")]
        [Authorize(Roles = "1")]
        public async Task<Result<PagedResult<ApplicationUserModel>>> GetListUser(int page = 1, int pageSize = 10, string searchTerm = "", bool isDeleted = false)
        {
            var result = await applicationUserService.GetAllAsync(page, pageSize, searchTerm);
            return result;
        }

        [HttpGet("{id:guid}")]
        public async Task<Result<ApplicationUserModel>> GetById(Guid id)
        {
            var result = await applicationUserService.GetByIdAsync(id);
            return result;
        }

        [HttpPut("{id:guid}")]
        public async Task<Result<ApplicationUserModel>> UpdateById(Guid id, ApplicationUserModel model)
        {
            var result = await applicationUserService.UpdateAsync(model);
            return result;
        }

        [HttpDelete("{id:guid}")]
        public async Task<Result> DeleteById(Guid id)
        {
            var result = await applicationUserService.DeleteAsync(id);
            return result;
        }

    }
}
