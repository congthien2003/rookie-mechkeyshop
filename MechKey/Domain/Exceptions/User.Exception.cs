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

    public class UserImageHandleFailedException : UserException
    {
        public UserImageHandleFailedException()
        {
            Type = Enum.ExceptionType.HANDLED_FAILED;
            Message = "User Image handle failed";
        }
    }
}
