namespace Lodgify.Context
{
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
}