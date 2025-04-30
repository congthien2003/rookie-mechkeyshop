using Domain.Common;

namespace Domain.Exceptions
{
    public class CategoryException : BaseException
    {

    }

    public class CategoryNotFoundException : CategoryException
    {
        public CategoryNotFoundException()
        {
            Type = Enum.ExceptionType.NOT_FOUND;
            Message = "Category not found";
        }
    }

    public class CategoryHandleFailedException : CategoryException
    {
        public CategoryHandleFailedException()
        {
            Type = Enum.ExceptionType.HANDLED_FAILED;
            Message = "Category handle failed";
        }
    }

    public class CategoryValidateFailedException : CategoryException
    {
        public CategoryValidateFailedException()
        {
            Type = Enum.ExceptionType.VALIDATION_FAILED;
            Message = "Validate Category failed";
        }
    }

    public class CategoryInvalidDataException : CategoryException
    {
        public CategoryInvalidDataException()
        {
            Type = Enum.ExceptionType.VALIDATION_FAILED;
            Message = "Invalid data";
        }
    }
}
