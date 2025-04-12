using Application.Comoon;
using Application.Interfaces.IServices;
using AutoMapper;
using Domain.Entity;
using Domain.IRepositories;
using Shared.ViewModels;
using Shared.ViewModels.Auth;

namespace Application.Services
{
    public class ApplicationUserService : IApplicaionUserService
    {
        private readonly IApplicationUserRepository<ApplicationUser> applicationUserRepository;

        private readonly IMapper mapper;

        public ApplicationUserService(IApplicationUserRepository<ApplicationUser> applicationUserRepository,
            IMapper mapper)
        {
            this.applicationUserRepository = applicationUserRepository;
            this.mapper = mapper;
        }

        public async Task<Result<ApplicationUserModel>> AddAsync(RegisterModel user)
        {
            try
            {
                var entity = mapper.Map<RegisterModel, ApplicationUser>(user);
                var newEntity = await applicationUserRepository.CreateAsync(entity);
                return Result<ApplicationUserModel>.Success("Add success", mapper.Map<ApplicationUser, ApplicationUserModel>(newEntity));
            }
            catch (Exception ex)
            {
                throw new InvalidDataException(ex.Message.ToString());
            }
        }

        public Task<Result<ApplicationUserModel>> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Result<ApplicationUserModel>> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Result<ApplicationUserModel>> UpdateAsync(ApplicationUserModel user)
        {
            throw new NotImplementedException();
        }
    }
}
