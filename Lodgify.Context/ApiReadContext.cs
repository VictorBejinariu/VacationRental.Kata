using System;

namespace Lodgify.Context
{
    public class ApiReadContext<TOutput>
    {
        private ApiReadContext()
        {
        }
        
        public ExecutionContext Execution { get; private set; } = new ExecutionContext();

        private TOutput _data;
        public TOutput Data
        {
            get => Execution.IsSuccess?_data:throw new Exception("Execution Failed");
            private set => _data = value;
        }

        public ApiReadContext<TOutput> WithData(TOutput data)
        {
            _data = data;
            return this;
        }
        public static ApiReadContext<TOutput> New()
        {
            var context = new ApiReadContext<TOutput>();
            return context;
        }
    }

    public class ApiCreateContext<TInput,TOutput>
    {
        private TOutput _data;
        public ExecutionContext Execution { get; private set; } = new ExecutionContext();
        public TInput Input { get; private set; }

        public TOutput Data
        {
            get
            {
                if (!Execution.IsSuccess)
                {
                    throw new Exception("Execution Failed");
                }
                return _data;
            }
            private set => _data = value;
        }

        protected ApiCreateContext(TInput input)
        {
            Input = input;
        }

        public ApiCreateContext<TInput,TOutput> WithResult(TOutput data)
        {
            Data = data;
            return this;
        }

        public static ApiCreateContext<TInput, TOutput> From(TInput input)
        {
            var context = new ApiCreateContext<TInput, TOutput>(input);
            return context;
        }

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