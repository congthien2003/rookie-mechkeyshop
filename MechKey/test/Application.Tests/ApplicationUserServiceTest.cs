using Application.Services;
using Domain.Entity;
using Domain.Exceptions;
using Domain.IRepositories;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;
using Shared.Mapping.Interfaces;
using Shared.ViewModels.Auth;

namespace Application.Test
{
    public class ApplicationUserServiceTest
    {
        private readonly Mock<IApplicationUserRepository<ApplicationUser>> _mockRepository;
        private readonly Mock<ILogger<ApplicationUserService>> _mockLogger;
        private readonly Mock<IApplicationUserMapping> _mockMapping;
        private readonly ApplicationUserService _applicationUserService;
        private readonly CancellationToken _cancellationToken;

        public ApplicationUserServiceTest()
        {
            _mockRepository = new Mock<IApplicationUserRepository<ApplicationUser>>();
            _mockLogger = new Mock<ILogger<ApplicationUserService>>();
            _mockMapping = new Mock<IApplicationUserMapping>();

            _applicationUserService = new ApplicationUserService(
                _mockRepository.Object,
                _mockLogger.Object,
                _mockMapping.Object
            );

            _cancellationToken = new CancellationToken();
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteUserSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var applicationUser = new ApplicationUser { Id = userId, Name = "Test User" };

            _mockRepository.Setup(r => r.GetByIdAsync(userId, _cancellationToken)).ReturnsAsync(applicationUser);
            _mockRepository.Setup(r => r.DeleteAsync(applicationUser, _cancellationToken)).Returns(Task.CompletedTask);

            // Act
            var result = await _applicationUserService.DeleteAsync(userId, _cancellationToken);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Delete user success", result.Message);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowException_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetByIdAsync(userId, _cancellationToken)).ReturnsAsync((ApplicationUser)null);

            // Act & Assert
            await Assert.ThrowsAsync<UserNotFoundException>(() => _applicationUserService.DeleteAsync(userId, _cancellationToken));
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

            var queryable = users.AsQueryable().BuildMock();

            _mockRepository.Setup(r => r.GetAllAsync()).Returns(queryable);

            // Act
            var result = await _applicationUserService.GetAllAsync(1, 10, "", _cancellationToken);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var applicationUser = new ApplicationUser { Id = userId, Name = "Test User", Email = "test@example.com" };
            var applicationUserModel = new ApplicationUserModel
            {
                Id = applicationUser.Id,
                Name = applicationUser.Name,
                Email = applicationUser.Email,

            };

            _mockRepository.Setup(r => r.GetByIdAsync(userId, _cancellationToken)).ReturnsAsync(applicationUser);
            _mockMapping.Setup(m => m.ToApplicationUserModel(applicationUser)).Returns(applicationUserModel);
            // Act
            var result = await _applicationUserService.GetByIdAsync(userId, _cancellationToken);

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
            _mockRepository.Setup(r => r.GetByIdAsync(userId, _cancellationToken)).ReturnsAsync((ApplicationUser)null);

            // Act & Assert
            await Assert.ThrowsAsync<UserNotFoundException>(() => _applicationUserService.GetByIdAsync(userId, _cancellationToken));
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateUserSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var applicationUser = new ApplicationUser { Id = userId, Name = "Old Name", Email = "old@example.com" };
            var updatedUserModel = new ApplicationUserModel { Id = userId, Name = "New Name", Email = "new@example.com" };

            _mockRepository.Setup(r => r.GetByIdAsync(userId, _cancellationToken)).ReturnsAsync(applicationUser);
            _mockRepository.Setup(r => r.UpdateAsync(applicationUser, _cancellationToken)).ReturnsAsync(applicationUser);
            _mockMapping.Setup(m => m.ToApplicationUserModel(applicationUser)).Returns(updatedUserModel);

            // Act
            var result = await _applicationUserService.UpdateAsync(updatedUserModel, _cancellationToken);

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

            _mockRepository.Setup(r => r.GetByIdAsync(userId, _cancellationToken)).ReturnsAsync((ApplicationUser)null);

            // Act & Assert
            await Assert.ThrowsAsync<UserNotFoundException>(() => _applicationUserService.UpdateAsync(updatedUserModel, _cancellationToken));
        }

        [Fact]
        public async Task UpdateStatusEmailConfirm_ShouldReturnSuccess()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var applicationUser = new ApplicationUser { Id = userId, Name = "Old Name", Email = "old@example.com", IsEmailConfirmed = false };
            _mockRepository.Setup(r => r.GetByIdAsync(userId, _cancellationToken)).ReturnsAsync(applicationUser);

            // Act
            await _applicationUserService.UpdateEmailConfirmAsync(userId, _cancellationToken);

            // Assert
            Assert.True(applicationUser.IsEmailConfirmed);
            _mockRepository.Verify(r => r.GetByIdAsync(userId, _cancellationToken), Times.Once());
        }
    }
}
