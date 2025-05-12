using Application.Comoon;
using Application.Events;
using Application.Interfaces.IServices;
using Application.Services.Common;
using Application.Validators;
using Domain.Entity;
using Domain.Exceptions;
using Domain.IRepositories;
using EventBus;
using Shared.Mapping.Interfaces;
using Shared.ViewModels.Auth;
namespace Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IApplicationUserRepository<ApplicationUser> applicationUserRepository;
        private readonly IApplicationUserMapping applicationUserMapping;
        private readonly IEventBus eventBus;

        public AuthenticationService(IApplicationUserRepository<ApplicationUser> _applicationUserRepository,
            IEventBus eventBus,
            IApplicationUserMapping applicationUserMapping)
        {
            applicationUserRepository = _applicationUserRepository;
            this.eventBus = eventBus;
            this.applicationUserMapping = applicationUserMapping;
        }

        public async Task<Result<ApplicationUserModel>> Login(LoginModel model, CancellationToken cancellationToken = default)
        {
            AuthValidator.ValidateLogin(model);

            var user = await applicationUserRepository.GetByEmailAsync(model.Email, cancellationToken);
            if (user == null)
            {
                throw new UserInvalidLoginException();
            }

            // Lấy salt từ DB và hash lại password nhập vào
            var salt = Convert.FromBase64String(user.Salting);
            var hashedInputPassword = Hashing.VerifyPassword(model.Password, user.Password, salt);

            // So sánh password
            if (!hashedInputPassword)
                throw new UserInvalidLoginException();

            if (!user.IsEmailConfirmed)
                throw new UserNotConfirmEmailException();

            if (user.IsDeleted)
                throw new UserIsDeletedException();

            return Result<ApplicationUserModel>.Success("Login success", applicationUserMapping.ToApplicationUserModel(user));
        }

        public async Task<Result> Register(RegisterModel model, CancellationToken cancellationToken = default)
        {
            AuthValidator.ValidateRegister(model);
            // Check phone and email exists
            var checkPhoneExist = applicationUserRepository.CheckPhoneExists(model.Phones)
                ? throw new UserPhoneExistsException() : "";

            var checkEmailExist = await applicationUserRepository.GetByEmailAsync(model.Email, cancellationToken);
            if (checkEmailExist != null)
                throw new UserEmailExistsException();

            // Map
            var entity = new ApplicationUser
            {
                Email = model.Email,
                Password = model.Password,
            };

            entity.Password = Hashing.HashPasword(entity.Password, out var salt);
            entity.Salting = Convert.ToBase64String(salt);

            var newEntity = await applicationUserRepository.CreateAsync(entity, cancellationToken);

            // Publish Event
            await eventBus.PublishAsync(new RegisterSuccessEvent
            {
                UserId = newEntity.Id,
                Email = newEntity.Email,
            }, cancellationToken);

            return Result.Success("Register success");
        }
    }
}
