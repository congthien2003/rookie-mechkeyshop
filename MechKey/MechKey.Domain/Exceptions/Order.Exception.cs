using Domain.Common;
using Domain.Enum;

namespace Domain.Exceptions
{
    public class OrderException : BaseException
    {
    }

    public class OrderNotFoundException : OrderException
    {
        public OrderNotFoundException()
        {
            Type = ExceptionType.NOT_FOUND;
            Message = "Order not found";
        }
    }

    public class OrderHandleFailedException : OrderException
    {
        public OrderHandleFailedException()
        {
            Type = ExceptionType.HANDLED_FAILED;
            Message = "Handle Order failed";
        }
    }

    public class OrderValidateFailedException : OrderException
    {
        public OrderValidateFailedException()
        {
            Type = ExceptionType.HANDLED_FAILED;
            Message = "Validation Order failed";
        }
    }
}
