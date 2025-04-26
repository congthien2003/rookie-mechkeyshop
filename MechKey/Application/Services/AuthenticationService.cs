using Application.Comoon;
using Application.Events;
using Application.Interfaces.IApiClient.MassTransit;
using Application.Interfaces.IServices;
using Application.Services.Common;
using AutoMapper;
using Domain.Entity;
using Domain.IRepositories;
using Shared.ViewModels.Auth;

namespace Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IApplicationUserRepository<ApplicationUser> applicationUserRepository;
        private readonly IMapper mapper;
        private readonly IEventBus eventBus;

        public AuthenticationService(IApplicationUserRepository<ApplicationUser> _applicationUserRepository,
            IMapper mapper,
            IEventBus eventBus)
        {
            applicationUserRepository = _applicationUserRepository;
            this.mapper = mapper;
            this.eventBus = eventBus;
        }

        public async Task<Result<ApplicationUserModel>> Login(LoginModel model)
        {
            try
            {
                // Tìm user theo email hoặc username
                var user = await applicationUserRepository.GetByEmailAsync(model.Email);
                if (user == null)
                {
                    throw new InvalidDataException("Invalid email or password");
                }

                // Lấy salt từ DB và hash lại password nhập vào
                var salt = Convert.FromBase64String(user.Salting);
                var hashedInputPassword = Hashing.VerifyPassword(model.Password, user.Password, salt);

                // So sánh password
                if (!hashedInputPassword)
                {
                    throw new InvalidDataException("Invalid email or password");

                }

                // Nếu đúng thì trả về success (có thể kèm token hoặc thông tin user nếu cần)
                return Result<ApplicationUserModel>.Success(
                    "Login success",
                    mapper.Map<ApplicationUser, ApplicationUserModel>(user)
                    );
            }
            catch (Exception ex)
            {
                throw new InvalidDataException("Invalid email or password");
            }
        }

        public async Task<Result> Register(RegisterModel model)
        {
            try
            {
                var entity = mapper.Map<RegisterModel, ApplicationUser>(model);
                entity.Password = Hashing.HashPasword(entity.Password, out var salt);
                entity.Salting = Convert.ToBase64String(salt);
                var newEntity = await applicationUserRepository.CreateAsync(entity);

                Console.WriteLine("Publish Event Register Success !!!");
                await eventBus.PublishAsync(new RegisterSuccessEvent
                {
                    UserId = newEntity.Id,
                    Email = newEntity.Email,
                });

                return Result.Success("Register success");
            }
            catch (Exception ex)
            {
                throw new InvalidDataException("Register failed");
            }
        }
    }
}
