using Application.Comoon;
using Application.Interfaces.IServices;
using Application.Services.Common;
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
                entity.Password = Hashing.HashPasword(entity.Password, out var salt);
                entity.Salting = Convert.ToBase64String(salt);
                var newEntity = await applicationUserRepository.CreateAsync(entity);
                return Result<ApplicationUserModel>.Success(
                    "Add success",
                    mapper.Map<ApplicationUser, ApplicationUserModel>(newEntity
                    ));
            }
            catch (Exception ex)
            {
                throw new Exception("Add failed");
            }
        }

        public async Task<Result> DeleteAsync(Guid id)
        {
            var entity = await applicationUserRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new Exception("Not found user to delete");
            }

            try
            {
                await applicationUserRepository.DeleteAsync(entity);
                return Result.Success("Delete user success");
            }
            catch (Exception ex)
            {
                throw new Exception("Delete user failed");
            }
        }

        public async Task<Result<ApplicationUserModel>> GetByIdAsync(Guid id)
        {
            var entity = await applicationUserRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new Exception("Not found user to delete");
            }
            return Result<ApplicationUserModel>.Success("Get user by id success",
                mapper.Map<ApplicationUser, ApplicationUserModel>(entity));
        }

        public async Task<Result<ApplicationUserModel>> UpdateAsync(ApplicationUserModel user)
        {
            try
            {
                var entity = mapper.Map<ApplicationUserModel, ApplicationUser>(user);
                var result = await applicationUserRepository.UpdateAsync(entity);
                return Result<ApplicationUserModel>.Success("Update user success",
                    mapper.Map<ApplicationUser, ApplicationUserModel>(entity));
            }
            catch (Exception ex)
            {
                throw new Exception("Update user failed");
            }
        }
    }
}
