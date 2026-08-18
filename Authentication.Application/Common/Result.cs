namespace Authentication.Application.Common
{
    public class Result
    {
        protected Result(
            bool isSuccess,
            Error? error)
        {
            if (isSuccess && error is not null)
                throw new ArgumentException();

            if (!isSuccess && error is null)
                throw new ArgumentException();

            IsSuccess = isSuccess;
            Error = error;
        }

        public bool IsSuccess { get; }

        public bool IsFailure => !IsSuccess;

        public Error? Error { get; }

        public static Result Success()
            => new(true, null);

        public static Result Failure(Error error)
            => new(false, error);
    }

    public sealed class Result<T> : Result
    {
        private Result(
            T value)
            : base(true, null)
        {
            Value = value;
        }

        private Result(
            Error error)
            : base(false, error)
        {
        }

        public T? Value { get; }

        public static Result<T> Success(
            T value)
            => new(value);

        public new static Result<T> Failure(
            Error error)
            => new(error);
    }
}
