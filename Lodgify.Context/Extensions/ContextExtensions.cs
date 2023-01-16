using System;

namespace Lodgify.Context.Extensions
{
    //TODO: To be deleted after trying to use
    public static class ContextExtensions
    {
        public static IContext<T> ThenTry<T>(this IContext<T> @this, Func<IContext<T>, IContext<T>> fn)
        {
            try
            {
                return @this.Handler.Execution.IsSuccess
                    ? fn(@this)
                    : @this;
            }
            catch (Exception e)
            {
                @this.Handler.With(Error.WithMessage(e.Message));
                return @this;
            }
        }

        public static IContext<T> AlwaysTry<T>(this IContext<T> @this, Func<IContext<T>, IContext<T>> fn)
        {
            try
            {
                return fn(@this);
            }
            catch (Exception e)
            {
                @this.Handler.With(Error.WithMessage(e.Message));
                return @this;
            }
        }
    }
}