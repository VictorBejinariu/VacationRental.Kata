using System;

namespace Lodgify.Context
{
    public interface IContext<TOutput>
    {
        RequestHandler<TOutput> Handler { get; set; }
    }

    public class RequestHandler<TOutput>
    {
        private RequestHandler()
        {
        }
        
        public ExecutionContext Execution { get; private set; } = new ExecutionContext();

        private TOutput _data;
        public TOutput Data
        {
            get => Execution.IsSuccess?_data:throw new Exception("Execution Failed");
            private set => _data = value;
        }

        public RequestHandler<TOutput> With(TOutput data)
        {
            _data = data;
            return this;
        }

        public RequestHandler<TOutput> With(Error error)
        {
            Execution.FailWith(error);
            return this;
        }
        public static RequestHandler<TOutput> New()
        {
            var context = new RequestHandler<TOutput>();
            return context;
        }
    }
}