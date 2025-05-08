using Domain.Common;

namespace Domain.Exceptions
{
    public class ProductRatingException : BaseException
    {

    }

    public class ProductRatingIsNull : ProductRatingException
    {
        public ProductRatingIsNull()
        {
            Type = Enum.ExceptionType.NOT_FOUND;
            Message = "Product raing is null";
        }
    }

    public class ProductRatingValidateFailedException : ProductRatingException
    {
        public ProductRatingValidateFailedException()
        {
            Type = Enum.ExceptionType.VALIDATION_FAILED;
            Message = "Validate Product failed";
        }

        public ProductRatingValidateFailedException(string message)
        {
            Type = Enum.ExceptionType.VALIDATION_FAILED;
            Message = message;
        }

    }

    public class ProductRatingInvalidDataException : ProductRatingException
    {
        public ProductRatingInvalidDataException()
        {
            Type = Enum.ExceptionType.VALIDATION_FAILED;
            Message = "Invalid data";
        }

        public ProductRatingInvalidDataException(string message)
        {
            Type = Enum.ExceptionType.VALIDATION_FAILED;
            Message = message;
        }
    }

    public class ProductRatingUserRatedException : ProductRatingException
    {
        public ProductRatingUserRatedException()
        {
            Type = Enum.ExceptionType.VALIDATION_FAILED;
            Message = "User has already rated this product.";
        }
    }
}
