using System;

namespace Lodgify.Dates
{
    public class DateInterval
    {
        public DateTime Start { get;  }
        public DateTime End { get; }

        public DateInterval(DateTime start, DateTime end)
        {
            if (start > end)
            {
                throw new ArgumentException(nameof(start));
            }

            Start = start;
            End = end;
        }

        public bool IsOverlapping(DateInterval anotherInterval)
        {
            return Start <= anotherInterval.Start && End > anotherInterval.Start.Date
                   || Start < anotherInterval.End && End >= anotherInterval.End
                   || Start > anotherInterval.Start && End < anotherInterval.End;
        }
    }
}