using Domain.Common;

namespace Domain.Exceptions
{
    public class UserException : BaseException
    {

    }

    public class UserNotFoundException : UserException
    {
        public UserNotFoundException()
        {
            Type = Enum.ExceptionType.NOT_FOUND;
            Message = "User not found";
        }
    }


    public class UserHandleFailedException : UserException
    {
        public UserHandleFailedException()
        {
            Type = Enum.ExceptionType.HANDLED_FAILED;
            Message = "User handle failed";
        }
    }

    public class UserValidateFailedException : UserException
    {
        public UserValidateFailedException()
        {
            Type = Enum.ExceptionType.VALIDATION_FAILED;
            Message = "Validate User failed";
        }

        public UserValidateFailedException(string message)
        {
            Type = Enum.ExceptionType.VALIDATION_FAILED;
            Message = message;
        }

    }

    public class UserEmailExistsException : UserException
    {
        public UserEmailExistsException()
        {
            Type = Enum.ExceptionType.VALIDATION_FAILED;
            Message = "Email is already in use";
        }

    }
    public class UserPhoneExistsException : UserException
    {
        public UserPhoneExistsException()
        {
            Type = Enum.ExceptionType.VALIDATION_FAILED;
            Message = "Phone is already in use";
        }

    }

    public class UserNotConfirmEmailException : UserException
    {
        public UserNotConfirmEmailException()
        {
            Type = Enum.ExceptionType.VALIDATION_FAILED;
            Message = "Your email is not confirmed. Please check your inbox to confirm it.";
        }

    }

    public class UserInvalidLoginException : UserException
    {
        public UserInvalidLoginException()
        {
            Type = Enum.ExceptionType.VALIDATION_FAILED;
            Message = "Invalid username or password";
        }
    }

    public class UserIsDeletedException : UserException
    {
        public UserIsDeletedException()
        {
            Type = Enum.ExceptionType.VALIDATION_FAILED;
            Message = "Account is deleted, please contact to admin";
        }
    }
}
