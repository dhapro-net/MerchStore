
namespace MerchStore.Application.Common
{
    public class Result
    {
        public bool IsSuccess { get; }
        public string Error { get; }
        public bool IsFailure => !IsSuccess;

        protected Result(bool isSuccess, string error)
        {
            if (isSuccess && !string.IsNullOrEmpty(error))
                throw new InvalidOperationException("A successful result cannot contain an error message");

            if (!isSuccess && string.IsNullOrEmpty(error))
                throw new InvalidOperationException("A failure result must contain an error message");

            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new(true, string.Empty);
        public static Result Failure(string error) => new(false, error);

        // Add these generic methods
        public static Result<T> Success<T>(T value) => Result<T>.Success(value);
        public static Result<T> Failure<T>(string error) => Result<T>.Failure(error);
    }

    public class Result<T> : Result
    {
        private readonly T _value;

        public T Value
        {
            get
            {
                if (!IsSuccess)
                    throw new InvalidOperationException("Cannot access the value of a failed result");

                return _value;
            }
        }

        protected internal Result(bool isSuccess, T value, string error)
            : base(isSuccess, error)
        {
            _value = value;
        }

        public static Result<T> Success(T value) => new(true, value, string.Empty);
        public static new Result<T> Failure(string error) => new(false, default!, error);
    }
}