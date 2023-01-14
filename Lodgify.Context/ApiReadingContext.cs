using System;

namespace Lodgify.Context
{
    public class ApiReadingContext<TOutput>
    {
        public ExecutionContext Execution { get; private set; } = new ExecutionContext();
        public TOutput Data { get; set; }
    }

    public class ExecutionContext
    {
        public bool IsSuccess { get; private set; } = true;
        public Error Error { get; private set; } = null;

        public void FailWith(Error error)
        {
            IsSuccess = false;
            Error = error;
        }
    }

    public class Error
    {
        public string Message { get; private set; }

        public void WithMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentException(nameof(message));
            }

            this.Message = message;
        }
    }
}