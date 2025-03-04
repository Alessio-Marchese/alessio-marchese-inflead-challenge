namespace Backend.Utility
{
    public class Result<T>
    {
        public bool IsOk { get; }
        public T? Data { get; }
        public string? ErrorMessage { get; }

        private Result(T data)
        {
            IsOk = true;
            Data = data;
        }

        private Result(string errorMessage)
        {
            IsOk = false;
            ErrorMessage = errorMessage;
        }

        public static Result<T> Success(T data)
        {
            return new Result<T>(data);
        }
        public static Result<T> Failure(string errorMessage)
        {
            return new Result<T>(errorMessage);
        }
    }
}
