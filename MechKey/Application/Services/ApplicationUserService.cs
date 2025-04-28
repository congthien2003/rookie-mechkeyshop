using Application.Comoon;
using Application.Interfaces.IServices;
using Application.Services.Common;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entity;
using Domain.Exceptions;
using Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Common;
using Shared.ViewModels.Auth;

namespace Application.Services
{
    public class ApplicationUserService : IApplicaionUserService
    {
        private readonly IApplicationUserRepository<ApplicationUser> applicationUserRepository;
        private readonly IMapper mapper;
        private readonly ILogger<ApplicationUserService> logger;

        public ApplicationUserService(
            IApplicationUserRepository<ApplicationUser> applicationUserRepository,
            IMapper mapper,
            ILogger<ApplicationUserService> logger)
        {
            this.applicationUserRepository = applicationUserRepository;
            this.mapper = mapper;
            this.logger = logger;
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
                    mapper.Map<ApplicationUser, ApplicationUserModel>(newEntity));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred in {Method}. User: {User}, Message: {Message}", nameof(AddAsync), user, ex.Message);
                throw new UserHandleFailedException();
            }
        }

        public async Task<bool> CheckEmailAddressExists(string email, string phone)
        {
            var emailExists = await applicationUserRepository.GetByEmailAsync(email);
            if (emailExists != null)
                throw new UserEmailExistsException();

            var phoneExists = applicationUserRepository.CheckPhoneExists(phone);
            if (phoneExists)
                throw new UserPhoneExistsException();

            return false;
        }

        public async Task<Result> DeleteAsync(Guid id)
        {
            try
            {
                var entity = await applicationUserRepository.GetByIdAsync(id);
                if (entity == null)
                {
                    throw new UserNotFoundException();
                }

                await applicationUserRepository.DeleteAsync(entity);
                return Result.Success("Delete user success");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred in {Method}. UserId: {UserId}, Message: {Message}", nameof(DeleteAsync), id, ex.Message);
                throw new UserHandleFailedException();
            }
        }

        public async Task<Result<PagedResult<ApplicationUserModel>>> GetAllAsync(int page = 1, int pageSize = 10, string searchTerm = "")
        {
            try
            {
                var query = applicationUserRepository.GetAllAsync();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(u => u.Name.Contains(searchTerm) || u.Email.Contains(searchTerm));
                }

                var totalCount = query.Count();
                var items = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ProjectTo<ApplicationUserModel>(mapper.ConfigurationProvider)
                    .ToListAsync();

                return Result<PagedResult<ApplicationUserModel>>.Success("Get List user success", new PagedResult<ApplicationUserModel>
                {
                    Items = items,
                    TotalItems = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred in {Method}. Page: {Page}, PageSize: {PageSize}, SearchTerm: {SearchTerm}, Message: {Message}", nameof(GetAllAsync), page, pageSize, searchTerm, ex.Message);
                throw new Exception(nameof(GetAllAsync));
            }
        }

        public async Task<Result<ApplicationUserModel>> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await applicationUserRepository.GetByIdAsync(id);
                if (entity == null)
                {
                    throw new UserNotFoundException();
                }
                return Result<ApplicationUserModel>.Success("Get user by id success",
                    mapper.Map<ApplicationUser, ApplicationUserModel>(entity));
            }
            catch (UserNotFoundException ex)
            {
                logger.LogWarning(ex, "User not found in {Method}. UserId: {UserId}", nameof(GetByIdAsync), id);
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred in {Method}. UserId: {UserId}, Message: {Message}", nameof(GetByIdAsync), id, ex.Message);
                throw new UserHandleFailedException();
            }
        }

        public async Task<Result<ApplicationUserModel>> UpdateAsync(ApplicationUserModel user)
        {
            try
            {
                var entity = await applicationUserRepository.GetByIdAsync(user.Id);
                if (entity == null)
                    throw new UserNotFoundException();

                entity.Name = user.Name;
                entity.Email = user.Email;
                entity.Address = user.Address;
                entity.Phones = user.Phones;
                entity.RoleId = user.RoleId;

                var result = await applicationUserRepository.UpdateAsync(entity);
                return Result<ApplicationUserModel>.Success("Update user success",
                    mapper.Map<ApplicationUser, ApplicationUserModel>(entity));
            }
            catch (UserNotFoundException ex)
            {
                logger.LogWarning(ex, "User not found in {Method}. UserId: {UserId}", nameof(UpdateAsync), user.Id);
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred in {Method}. UserId: {UserId}, Message: {Message}", nameof(UpdateAsync), user.Id, ex.Message);
                throw new UserHandleFailedException();
            }
        }

        public async Task UpdateEmailConfirmAsync(Guid id)
        {
            try
            {
                var user = await applicationUserRepository.GetByIdAsync(id) ?? throw new UserNotFoundException();
                user.ChangeEmailConfirm(true);

                await applicationUserRepository.UpdateAsync(user);
            }
            catch (UserNotFoundException ex)
            {
                logger.LogWarning(ex, "User not found in {Method}. UserId: {UserId}", nameof(UpdateEmailConfirmAsync), id);
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred in {Method}. UserId: {UserId}, Message: {Message}", nameof(UpdateEmailConfirmAsync), id, ex.Message);
                throw new UserHandleFailedException();
            }
        }
    }
}
