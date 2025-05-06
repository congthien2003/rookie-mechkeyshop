using Application.Comoon;
using Application.Interfaces.IServices;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;
using Shared.ViewModels.Auth;

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
        /*[Authorize(Roles = "1")]*/
        public async Task<Result<PagedResult<ApplicationUserModel>>> GetListUser(int page = 1, int pageSize = 10, string searchTerm = "", bool isDeleted = false, CancellationToken cancellationToken = default)
        {
            var result = await applicationUserService.GetAllAsync(page, pageSize, searchTerm, cancellationToken);
            return result;
        }

        [HttpGet("{id:guid}")]
        public async Task<Result<ApplicationUserModel>> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await applicationUserService.GetByIdAsync(id, cancellationToken);
            return result;
        }

        [HttpPut("{id:guid}")]
        public async Task<Result<ApplicationUserModel>> UpdateById(Guid id, ApplicationUserModel model, CancellationToken cancellationToken = default)
        {
            var result = await applicationUserService.UpdateAsync(model, cancellationToken);
            return result;
        }

        [HttpDelete("{id:guid}")]
        public async Task<Result> DeleteById(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await applicationUserService.DeleteAsync(id, cancellationToken);
            return result;
        }

    }
}
