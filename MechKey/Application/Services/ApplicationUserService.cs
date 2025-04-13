using Application.Comoon;
using Application.Interfaces.IServices;
using Application.Services.Common;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entity;
using Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using Shared.Common;
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

        public async Task<Result<PagedResult<ApplicationUserModel>>> GetAllAsync(int page = 1, int pageSize = 10, string searchTerm = "")
        {
            try
            {
                var query = await applicationUserRepository.GetAllAsync();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(u => u.Name.Contains(searchTerm));
                }

                var totalCount = query.Count();
                var items = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ProjectTo<ApplicationUserModel>(mapper.ConfigurationProvider) // AutoMapper
                    .ToListAsync();

                return Result<PagedResult<ApplicationUserModel>>.Success("Get List user success", new PagedResult<ApplicationUserModel>
                {
                    Items = items,
                    TotalItems = totalCount,
                    Page = page,
                    PageSize = pageSize
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Get list failed");
            }
        }

        public async Task<Result<ApplicationUserModel>> GetByIdAsync(Guid id)
        {
            var entity = await applicationUserRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new Exception("Not found user ");
            }
            return Result<ApplicationUserModel>.Success("Get user by id success",
                mapper.Map<ApplicationUser, ApplicationUserModel>(entity));
        }

        public async Task<Result<ApplicationUserModel>> UpdateAsync(ApplicationUserModel user)
        {
            try
            {
                var entity = await applicationUserRepository.GetByIdAsync(user.Id);
                if (entity == null)
                    throw new KeyNotFoundException("Not found user by id");

                entity.Name = user.Name;
                entity.Email = user.Email;
                entity.Address = user.Address;
                entity.Phones = user.Phones;
                entity.RoleId = user.RoleId;

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
