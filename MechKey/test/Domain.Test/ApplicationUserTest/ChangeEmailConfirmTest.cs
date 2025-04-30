using Domain.Entity;

namespace Domain.Test.ApplicationUserTest
{
    public class ChangeEmailConfirmTest
    {
        [Fact]
        public void Change_Email_Confirm_Test()
        {
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                RoleId = 2,
                Email = "test@gmail.com",
                IsEmailConfirmed = false,
            };

            const bool statusConfirmed = true;

            // Act
            user.ChangeEmailConfirm(statusConfirmed);

            // Assert
            Assert.True(user.IsEmailConfirmed);
        }
    }
}
