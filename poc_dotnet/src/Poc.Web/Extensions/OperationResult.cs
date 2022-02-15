using System;

namespace Poc.Web.Extensions
{
    public struct OperationResult<T>
    {
        public T Result { get; set; }
        public Exception Exception { get; }
        public bool IsSuccess { get; }

        public OperationResult(T result)
        {
            IsSuccess = true;
            Exception = null;
            Result = result;
        }

        public OperationResult(Exception exception)
        {
            Exception = exception;
            IsSuccess = false;
            Result = default;
        }

        public static implicit operator OperationResult<T>(T result)
            => new OperationResult<T>(result);

        public static implicit operator OperationResult<T>(Exception exception)
            => new OperationResult<T>(exception);
    }

    public struct OperationResult
    {
        public Exception Exception { get; }
        public bool IsSuccess { get; }

        public OperationResult(bool success)
        {
            IsSuccess = success;
            Exception = null;
        }

        public OperationResult(Exception exception)
        {
            Exception = exception;
            IsSuccess = false;
        }

        public static OperationResult<T> Success<T>(T result)
            => new OperationResult<T>(result);

        public static OperationResult<T> Error<T>(Exception exception)
            => new OperationResult<T>(exception);

        public static implicit operator OperationResult(Exception exception)
            => new OperationResult(exception);
    }
}
