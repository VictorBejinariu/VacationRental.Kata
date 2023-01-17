using System;
using System.Threading.Tasks;
using VacationRental.Domain;

namespace VacationRental.Application.Abstractions
{
    public interface IBookingActualStartHydrator
    {
        string Key { get; }
        Task<DateTime> Hydrate(Booking booking, DateTime previousValue);
    }
}