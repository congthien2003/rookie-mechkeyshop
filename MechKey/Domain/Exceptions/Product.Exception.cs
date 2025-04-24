using Domain.Common;

namespace Domain.Exceptions
{

    public class ProductException : BaseException
    {

    }

    public class ProductNotFoundException : ProductException
    {
        public ProductNotFoundException()
        {
            Type = Enum.ExceptionType.NOT_FOUND;
            Message = "Product not found";
        }
    }

    public class ProductHandleFailedException : ProductException
    {
        public ProductHandleFailedException()
        {
            Type = Enum.ExceptionType.HANDLED_FAILED;
            Message = "Product handle failed";
        }
    }

    public class ProductValidateFailedException : ProductException
    {
        public ProductValidateFailedException()
        {
            Type = Enum.ExceptionType.VALIDATION_FAILED;
            Message = "Validate Product failed";
        }

    }

    public class ProductImageHandleFailedException : ProductException
    {
        public ProductImageHandleFailedException()
        {
            Type = Enum.ExceptionType.HANDLED_FAILED;
            Message = "Product Image handle failed";
        }
    }


}
