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
}
