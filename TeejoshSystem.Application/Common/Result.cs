

namespace TeejoshSystem.Application.Common
{
    public class Result
    {
        public bool IsSuccess { get; }
        public string? Error { get; }

        protected Result(bool success, string? error)
        {
            IsSuccess = success;
            Error = error;
        }

        public static Result Success() => new(true, null);
        public static Result Failure(string error) => new(false, error);

        // Factory para crear Result<T> desde Result - evita repetir new Result<T>(...)
        public static Result<T> Success<T>(T value) => new(value, true, null);
        public static Result<T> Failure<T>(string error) => new(default!, false, error);
    }

    public class Result<T> : Result
    {
        public T Value { get; }

        internal Result(T value, bool success, string? error)
            : base(success, error)
        {
            Value = value;
        }
    }
}
