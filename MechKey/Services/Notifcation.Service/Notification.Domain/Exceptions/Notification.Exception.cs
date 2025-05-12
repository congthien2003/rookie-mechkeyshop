using Notification.Domain.Enum;

namespace Notification.Domain.Exceptions
{
    public class NotificationException : Exception
    {
        public ExceptionType Type { get; set; }
        public string Message { get; set; }
    }

    public class NotifcationHandleFailException : NotificationException
    {
        public NotifcationHandleFailException()
        {
            Type = ExceptionType.HANDLED_FAILED;
            Message = "Notification handled fail";
        }

        public NotifcationHandleFailException(string messsage)
        {
            Type = ExceptionType.HANDLED_FAILED;
            Message = messsage;
        }
    }

    public class NotifcationNotFoundException : NotificationException
    {
        public NotifcationNotFoundException()
        {
            Type = ExceptionType.NOT_FOUND;
            Message = "Notification not found";
        }

        public NotifcationNotFoundException(string messsage)
        {
            Type = ExceptionType.HANDLED_FAILED;
            Message = messsage;
        }
    }

    public class NotifcationValidatedFailException : NotificationException
    {
        public NotifcationValidatedFailException()
        {
            Type = ExceptionType.VALIDATION_FAILED;
            Message = "Notification validated fail";
        }

        public NotifcationValidatedFailException(string messsage)
        {
            Type = ExceptionType.HANDLED_FAILED;
            Message = messsage;
        }
    }
}
