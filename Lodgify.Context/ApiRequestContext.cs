using System;

namespace Lodgify.Context
{
    public class ApiRequestContext<TOutput>
    {
        private ApiRequestContext()
        {
        }
        
        public ExecutionContext Execution { get; private set; } = new ExecutionContext();

        private TOutput _data;
        public TOutput Data
        {
            get => Execution.IsSuccess?_data:throw new Exception("Execution Failed");
            private set => _data = value;
        }

        public ApiRequestContext<TOutput> With(TOutput data)
        {
            _data = data;
            return this;
        }

        public ApiRequestContext<TOutput> With(Error error)
        {
            Execution.FailWith(error);
            return this;
        }
        public static ApiRequestContext<TOutput> New()
        {
            var context = new ApiRequestContext<TOutput>();
            return context;
        }
    }
}