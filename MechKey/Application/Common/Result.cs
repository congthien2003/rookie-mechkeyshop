namespace Application.Comoon
{
    public interface IResult
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
    }

    public interface IResult<T>
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public T Data { get; set; }
    }

    public class Result : IResult
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }

        private Result(bool success, string? message)
        {
            IsSuccess = success;
            Message = message;
        }

        public static Result Success(string? message) => new Result(true, message);
        public static Result Failure(string? message) => new Result(false, message);
    }

    public class Result<T> : IResult
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public T Data { get; set; }
        private Result(bool success, string? message, T data)
        {
            IsSuccess = success;
            Message = message;
            Data = data;
        }
        public static Result<T> Success(string? message, T data) => new Result<T>(true, message, data);
        public static Result<T> Failure(string? message, T data) => new Result<T>(false, message, data);
    }

}