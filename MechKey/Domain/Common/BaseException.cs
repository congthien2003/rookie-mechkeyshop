using Domain.Enum;

namespace Domain.Common
{
    public abstract class BaseException : Exception
    {
        public ExceptionType Type { get; set; }
        public string Message { get; set; }
    }
}
