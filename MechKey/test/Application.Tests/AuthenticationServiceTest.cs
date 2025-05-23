﻿using Application.Events;
using Application.Services;
using Application.Services.Common;
using Domain.Entity;
using Domain.Exceptions;
using Domain.IRepositories;
using EventBus;
using Moq;
using Shared.Mapping.Interfaces;
using Shared.ViewModels.Auth;

namespace Application.Test
{
    public class AuthenticationServiceTest
    {
        private readonly Mock<IApplicationUserRepository<ApplicationUser>> _mockUserRepository;
        private readonly Mock<IEventBus> _mockEventBus;
        private readonly AuthenticationService _authenticationService;
        private readonly CancellationToken _cancellationToken;
        private readonly Mock<IApplicationUserMapping> _mockUserMapping;
        private static RegisterModel CreateRegisterModel() => new RegisterModel
        {
            Id = Guid.NewGuid(),
            Name = "Name",
            Email = "test@example.com",
            Password = "password123",
            ConfirmPassword = "password123",
            Phones = "1234567890",
            Address = "TPHCM",
            RoleId = 2,
        };
        public AuthenticationServiceTest()
        {
            _mockUserRepository = new Mock<IApplicationUserRepository<ApplicationUser>>();
            _mockEventBus = new Mock<IEventBus>();
            _mockUserMapping = new Mock<IApplicationUserMapping>();
            _authenticationService = new AuthenticationService(
                _mockUserRepository.Object,
                _mockEventBus.Object,
                _mockUserMapping.Object
            );

            _cancellationToken = new CancellationToken();
        }

        [Fact]
        public async Task Login_ShouldReturnSuccess_WhenCredentialsAreValid()
        {
            // Arrange
            var loginModel = new LoginModel { Email = "test@example.com", Password = "password123" };
            var applicationUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                Password = Hashing.HashPasword("password123", out var salt),
                Salting = Convert.ToBase64String(salt),
                IsEmailConfirmed = true
            };

            _mockUserRepository.Setup(r => r.GetByEmailAsync(loginModel.Email, _cancellationToken)).ReturnsAsync(applicationUser);

            // Act
            var result = await _authenticationService.Login(loginModel, _cancellationToken);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Login success", result.Message);
            Assert.Equal(applicationUser.Email, result.Data.Email);
        }

        [Fact]
        public async Task Login_ShouldThrowException_WhenUserNotFound()
        {
            // Arrange
            var loginModel = new LoginModel { Email = "nonexistent@example.com", Password = "password123" };

            _mockUserRepository.Setup(r => r.GetByEmailAsync(loginModel.Email, _cancellationToken)).ReturnsAsync((ApplicationUser)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UserInvalidLoginException>(() => _authenticationService.Login(loginModel, _cancellationToken));

            Assert.Equal("Invalid username or password", exception.Message);
        }

        [Fact]
        public async Task Login_ShouldThrowException_WhenPasswordIsInvalid()
        {
            // Arrange
            var loginModel = new LoginModel { Email = "test@example.com", Password = "wrongpassword" };
            var applicationUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                Password = Hashing.HashPasword("password123", out var salt),
                Salting = Convert.ToBase64String(salt),
                IsEmailConfirmed = true
            };

            _mockUserRepository.Setup(r => r.GetByEmailAsync(loginModel.Email, _cancellationToken)).ReturnsAsync(applicationUser);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UserInvalidLoginException>(() => _authenticationService.Login(loginModel, _cancellationToken));

            Assert.Equal("Invalid username or password", exception.Message);

        }

        [Fact]
        public async Task Login_ShouldThrowException_WhenEmailIsNotConfirmed()
        {
            // Arrange
            var loginModel = new LoginModel { Email = "test@example.com", Password = "password123" };
            var applicationUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                Password = Hashing.HashPasword("password123", out var salt),
                Salting = Convert.ToBase64String(salt),
                IsEmailConfirmed = false
            };

            _mockUserRepository.Setup(r => r.GetByEmailAsync(loginModel.Email, _cancellationToken)).ReturnsAsync(applicationUser);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UserNotConfirmEmailException>(() => _authenticationService.Login(loginModel, _cancellationToken));

            Assert.Equal("Your email is not confirmed. Please check your inbox to confirm it.", exception.Message);

        }

        [Fact]
        public async Task Register_ShouldReturnSuccess_WhenDataIsValid()
        {
            // Arrange
            var registerModel = CreateRegisterModel();

            var applicationUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = registerModel.Email,
                Password = Hashing.HashPasword(registerModel.Password, out var salt),
                Salting = Convert.ToBase64String(salt),
                Phones = registerModel.Phones
            };

            _mockUserRepository.Setup(r => r.GetByEmailAsync(registerModel.Email, _cancellationToken)).ReturnsAsync((ApplicationUser)null);
            _mockUserRepository.Setup(r => r.CheckPhoneExists(registerModel.Phones)).Returns(false);
            _mockUserRepository.Setup(r => r.CreateAsync(It.IsAny<ApplicationUser>(), _cancellationToken)).ReturnsAsync(applicationUser);
            _mockEventBus.Setup(e => e.PublishAsync(It.IsAny<RegisterSuccessEvent>(), _cancellationToken)).Returns(Task.CompletedTask);

            // Act
            var result = await _authenticationService.Register(registerModel, _cancellationToken);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Register success", result.Message);
        }

        [Fact]
        public async Task Register_ShouldThrowException_WhenEmailAlreadyExists()
        {
            // Arrange
            var registerModel = CreateRegisterModel();

            var existingUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = registerModel.Email
            };

            _mockUserRepository.Setup(r => r.GetByEmailAsync(registerModel.Email, _cancellationToken)).ReturnsAsync(existingUser);

            // Act & Assert
            await Assert.ThrowsAsync<UserEmailExistsException>(() => _authenticationService.Register(registerModel, _cancellationToken));
        }

        [Fact]
        public async Task Register_ShouldThrowException_WhenPhoneAlreadyExists()
        {
            // Arrange
            var registerModel = CreateRegisterModel();

            _mockUserRepository.Setup(r => r.GetByEmailAsync(registerModel.Email, _cancellationToken)).ReturnsAsync((ApplicationUser)null);
            _mockUserRepository.Setup(r => r.CheckPhoneExists(registerModel.Phones)).Returns(true);

            // Act & Assert
            await Assert.ThrowsAsync<UserPhoneExistsException>(() => _authenticationService.Register(registerModel, _cancellationToken));
        }
    }
}
