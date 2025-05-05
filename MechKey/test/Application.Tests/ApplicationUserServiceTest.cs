using Application.Services;
using AutoMapper;
using Domain.Entity;
using Domain.Exceptions;
using Domain.IRepositories;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.ViewModels.Auth;

namespace Application.Test
{
    public class ApplicationUserServiceTest
    {
        private readonly Mock<IApplicationUserRepository<ApplicationUser>> _mockRepository;
        private readonly IMapper _mapper; // Use actual mapper instance
        private readonly Mock<ILogger<ApplicationUserService>> _mockLogger;
        private readonly ApplicationUserService _applicationUserService;

        public ApplicationUserServiceTest()
        {
            _mockRepository = new Mock<IApplicationUserRepository<ApplicationUser>>();

            // Initialize AutoMapper with AutoMapperProfile
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ApplicationUser, ApplicationUserModel>().ReverseMap();
                cfg.CreateMap<ApplicationUser, RegisterModel>().ReverseMap();
            });
            _mapper = mapperConfig.CreateMapper();

            _mockLogger = new Mock<ILogger<ApplicationUserService>>();

            _applicationUserService = new ApplicationUserService(
                _mockRepository.Object,
                _mapper,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteUserSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var applicationUser = new ApplicationUser { Id = userId, Name = "Test User" };

            _mockRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(applicationUser);
            _mockRepository.Setup(r => r.DeleteAsync(applicationUser)).Returns(Task.CompletedTask);

            // Act
            var result = await _applicationUserService.DeleteAsync(userId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Delete user success", result.Message);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowException_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((ApplicationUser)null);

            // Act & Assert
            await Assert.ThrowsAsync<UserNotFoundException>(() => _applicationUserService.DeleteAsync(userId));
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPaginatedUsers()
        {
            // Arrange
            var users = new List<ApplicationUser>
                {
                    new ApplicationUser { Id = Guid.NewGuid(), Name = "User 1", Email = "user1@example.com" },
                    new ApplicationUser { Id = Guid.NewGuid(), Name = "User 2", Email = "user2@example.com" }
                };

            _mockRepository.Setup(r => r.GetAllAsync()).Returns(users.AsQueryable());

            // Act
            var result = await _applicationUserService.GetAllAsync(1, 10);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Data.Items.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var applicationUser = new ApplicationUser { Id = userId, Name = "Test User", Email = "test@example.com" };

            _mockRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(applicationUser);

            // Act
            var result = await _applicationUserService.GetByIdAsync(userId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Test User", result.Data.Name);
            Assert.Equal("test@example.com", result.Data.Email);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowException_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((ApplicationUser)null);

            // Act & Assert
            await Assert.ThrowsAsync<UserNotFoundException>(() => _applicationUserService.GetByIdAsync(userId));
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateUserSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var applicationUser = new ApplicationUser { Id = userId, Name = "Old Name", Email = "old@example.com" };
            var updatedUserModel = new ApplicationUserModel { Id = userId, Name = "New Name", Email = "new@example.com" };

            _mockRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(applicationUser);
            _mockRepository.Setup(r => r.UpdateAsync(applicationUser)).ReturnsAsync(applicationUser);

            // Act
            var result = await _applicationUserService.UpdateAsync(updatedUserModel);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Update user success", result.Message);
            Assert.Equal("New Name", result.Data.Name);
            Assert.Equal("new@example.com", result.Data.Email);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowException_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var updatedUserModel = new ApplicationUserModel { Id = userId, Name = "New Name", Email = "new@example.com" };

            _mockRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((ApplicationUser)null);

            // Act & Assert
            await Assert.ThrowsAsync<UserNotFoundException>(() => _applicationUserService.UpdateAsync(updatedUserModel));
        }

        [Fact]
        public async Task UpdateStatusEmailConfirm_ShouldReturnSuccess()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var applicationUser = new ApplicationUser { Id = userId, Name = "Old Name", Email = "old@example.com", IsEmailConfirmed = false };
            _mockRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(applicationUser);

            // Act
            await _applicationUserService.UpdateEmailConfirmAsync(userId);

            // Arrange
            Assert.True(applicationUser.IsEmailConfirmed);
            _mockRepository.Verify(r => r.GetByIdAsync(userId), Times.Once());
        }
    }
}
