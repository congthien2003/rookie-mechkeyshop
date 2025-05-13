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

        public ProductValidateFailedException(string message)
        {
            Type = Enum.ExceptionType.VALIDATION_FAILED;
            Message = message;
        }

    }

    public class ProductDuplicateNameFailedException : ProductException
    {
        public ProductDuplicateNameFailedException()
        {
            Type = Enum.ExceptionType.VALIDATION_FAILED;
            Message = "Product's name is already in use";
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

    public class ProductInvalidDataException : ProductException
    {
        public ProductInvalidDataException()
        {
            Type = Enum.ExceptionType.HANDLED_FAILED;
            Message = "Invalid data";
        }

        public ProductInvalidDataException(string message)
        {
            Type = Enum.ExceptionType.VALIDATION_FAILED;
            Message = message;
        }
    }

    public class InsufficientStockException : ProductException
    {
        public string ProductId { get; }

        public InsufficientStockException(string productId, int requestedQuantity, int availableQuantity)
        {
            ProductId = productId;
            Type = Enum.ExceptionType.VALIDATION_FAILED;
            Message = $"Insufficient stock for product '{productId}'. Requested: {requestedQuantity}, Available: {availableQuantity}.";
        }
    }

}
