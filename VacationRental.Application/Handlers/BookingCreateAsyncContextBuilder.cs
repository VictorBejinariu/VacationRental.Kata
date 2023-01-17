using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VacationRental.Application.Contracts;

namespace VacationRental.Application.Handlers
{
    public class BookingCreateAsyncContextBuilder
    {
        private BookingCreateContext _context;

        private readonly ICollection<AsyncStep<BookingCreateContext>> _steps = new List<AsyncStep<BookingCreateContext>>();
        
        protected BookingCreateAsyncContextBuilder(BookingCreate input)
        {
            _context = BookingCreateContext.From(input);
        }

        public BookingCreateAsyncContextBuilder AndThenAlways(Func<BookingCreateContext, Task<BookingCreateContext>> action)
        {
            _steps.Add(new AsyncStep<BookingCreateContext>(action));
            return this;
        }

        public BookingCreateAsyncContextBuilder AndThenTry(Func<BookingCreateContext, Task<BookingCreateContext>> action)
        {
            _steps.Add(new AsyncStep<BookingCreateContext>(action, (c)=>Task.FromResult(c.Handler.Execution.IsSuccess)));
            return this;
        }

        public async Task<BookingCreateContext> Run()
        {
            foreach (var step in _steps)
            {
                if (!await step.ShouldRun(_context))
                {
                    continue;
                }
                await step.Run(_context);
            }
            return _context;
        }

        public static BookingCreateAsyncContextBuilder From(BookingCreate input)
        {
            var newBuilder = new BookingCreateAsyncContextBuilder(input);
            return newBuilder;
        }
    }

    public class AsyncStep<T>
    {
        public Func<BookingCreateContext, Task<BookingCreateContext>> Run { get; private set; }
        public Func<BookingCreateContext, Task<bool>> ShouldRun { get; private set; }

        public AsyncStep(Func<BookingCreateContext, Task<BookingCreateContext>> action,
            Func<BookingCreateContext, Task<bool>> condition)
        {
            Run = action;
            ShouldRun = condition;
        }

        public AsyncStep(Func<BookingCreateContext, Task<BookingCreateContext>> action)
        {
            Run = action;
            ShouldRun = _ => Task.FromResult<bool>(true);
        }
        
    }
}