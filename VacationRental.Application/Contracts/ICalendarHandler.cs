using System;
using System.Threading.Tasks;
using Lodgify.Context;

namespace VacationRental.Application.Contracts
{
    public interface ICalendarHandler
    {
        Task<RequestHandler<RentalCalendar>> GetRentalCalendarByInterval(int rentalId,DateTime start, int nights);
    }
}