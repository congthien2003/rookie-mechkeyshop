using Domain.Exceptions;
using Shared.ViewModels.Auth;

namespace Application.Validators
{
    public class AuthValidator
    {
        public static void ValidateLogin(LoginModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (string.IsNullOrWhiteSpace(model.Email))
                throw new UserValidateFailedException("Email is required");

            if (string.IsNullOrWhiteSpace(model.Password))
                throw new UserValidateFailedException("Password is required");

            // Optional: check basic email format
            if (!model.Email.Contains('@'))
                throw new UserValidateFailedException("Email format is invalid");
        }

        public static void ValidateRegister(RegisterModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (Guid.Equals(model.Id, Guid.Empty))
                throw new UserValidateFailedException("User Id is required");

            if (string.IsNullOrWhiteSpace(model.Name))
                throw new UserValidateFailedException("Name is required");

            if (string.IsNullOrWhiteSpace(model.Email))
                throw new UserValidateFailedException("Email is required");

            if (!model.Email.Contains('@'))
                throw new UserValidateFailedException("Email format is invalid");

            if (string.IsNullOrWhiteSpace(model.Password))
                throw new UserValidateFailedException("Password is required");

            if (model.Password.Length < 6)
                throw new UserValidateFailedException("Password must be at least 6 characters");

            if (model.Password != model.ConfirmPassword)
                throw new UserValidateFailedException("Passwords do not match");

            if (string.IsNullOrWhiteSpace(model.Phones))
                throw new UserValidateFailedException("Phone number is required");

            if (string.IsNullOrWhiteSpace(model.Address))
                throw new UserValidateFailedException("Address is required");

            if (model.RoleId <= 0)
                throw new UserValidateFailedException("RoleId must be greater than 0");
        }
    }
}
