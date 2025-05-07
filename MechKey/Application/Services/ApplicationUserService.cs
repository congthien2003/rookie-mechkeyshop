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

        public async Task<Result<ApplicationUserModel>> AddAsync(RegisterModel user, CancellationToken cancellationToken = default)
        {
            var entity = mapper.Map<RegisterModel, ApplicationUser>(user);
            entity.Password = Hashing.HashPasword(entity.Password, out var salt);
            entity.Salting = Convert.ToBase64String(salt);
            var newEntity = await applicationUserRepository.CreateAsync(entity, cancellationToken);
            return Result<ApplicationUserModel>.Success(
                "Add success",
                mapper.Map<ApplicationUser, ApplicationUserModel>(newEntity));
        }

        public async Task<bool> CheckEmailAddressExists(string email, string phone, CancellationToken cancellationToken = default)
        {
            var emailExists = await applicationUserRepository.GetByEmailAsync(email, cancellationToken);
            if (emailExists != null)
                throw new UserEmailExistsException();

            var phoneExists = applicationUserRepository.CheckPhoneExists(phone);
            if (phoneExists)
                throw new UserPhoneExistsException();

            return false;
        }

        public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await applicationUserRepository.GetByIdAsync(id, cancellationToken);
            if (entity == null)
            {
                throw new UserNotFoundException();
            }

            await applicationUserRepository.DeleteAsync(entity, cancellationToken);
            return Result.Success("Delete user success");
        }

        public async Task<Result<PagedResult<ApplicationUserModel>>> GetAllAsync(int page = 1, int pageSize = 10, string searchTerm = "", CancellationToken cancellationToken = default)
        {
            var query = applicationUserRepository.GetAllAsync();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(u => u.Name.Contains(searchTerm) || u.Email.Contains(searchTerm));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<ApplicationUserModel>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return Result<PagedResult<ApplicationUserModel>>.Success("Get List user success", new PagedResult<ApplicationUserModel>
            {
                Items = items,
                TotalItems = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            });
        }

        public async Task<Result<ApplicationUserModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await applicationUserRepository.GetByIdAsync(id, cancellationToken);
            if (entity == null)
                throw new UserNotFoundException();

            return Result<ApplicationUserModel>.Success("Get user by id success",
                mapper.Map<ApplicationUser, ApplicationUserModel>(entity));
        }

        public async Task<Result<ApplicationUserModel>> UpdateAsync(ApplicationUserModel user, CancellationToken cancellationToken = default)
        {
            var entity = await applicationUserRepository.GetByIdAsync(user.Id, cancellationToken);
            if (entity == null)
                throw new UserNotFoundException();

            entity.Name = user.Name;
            entity.Email = user.Email;
            entity.Address = user.Address;
            entity.Phones = user.Phones;
            entity.RoleId = user.RoleId;
            entity.IsDeleted = user.IsDeleted;
            var result = await applicationUserRepository.UpdateAsync(entity, cancellationToken);
            return Result<ApplicationUserModel>.Success("Update user success",
                mapper.Map<ApplicationUser, ApplicationUserModel>(entity));
        }

        public async Task UpdateEmailConfirmAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var user = await applicationUserRepository.GetByIdAsync(id, cancellationToken) ?? throw new UserNotFoundException();
            user.ChangeEmailConfirm(true);

            await applicationUserRepository.UpdateAsync(user, cancellationToken);
        }
    }
}
