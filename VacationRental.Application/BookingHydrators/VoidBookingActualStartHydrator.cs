using System;
using System.Threading.Tasks;
using VacationRental.Application.Abstractions;
using VacationRental.Domain;

namespace VacationRental.Application.BookingHydrators
{
    public class VoidBookingActualStartHydrator:IBookingActualStartHydrator
    {
        public string Key { get; } = "void";
        public Task<DateTime> Hydrate(Booking booking, DateTime previousValue)
        {
            return Task.FromResult(previousValue);
        }
    }
}