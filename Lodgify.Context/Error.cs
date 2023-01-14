using System;

namespace Lodgify.Context
{
    public class Error
    {
        private Error(string message)
        {
            Message = message;
        }
        
        public string Message { get; private set; }

        public static Error WithMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentException(nameof(message));
            }

            var error = new Error(message);
            return error;
        }
    }
}