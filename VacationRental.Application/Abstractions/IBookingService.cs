using System.Collections.Generic;
using System.Threading.Tasks;
using VacationRental.Domain;

namespace VacationRental.Application.Abstractions
{
    public interface IBookingService
    {
        Task<ICollection<Booking>> Get();
        Task<Booking> GetById(int bookingId);
        Task<bool> Create(Booking input);
        Task<ICollection<Booking>> GetByUnitId(int unitId);
    }
}