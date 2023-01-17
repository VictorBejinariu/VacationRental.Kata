using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using VacationRental.Domain;

namespace VacationRental.Application.Abstractions
{
    public interface IBookingRepository
    {
        Task<ICollection<Booking>> Get();
        Task<Booking> GetById(int bookingId);
        Task<bool> Create(Booking input);
        Task<ICollection<Booking>> GetByUnitId(int unitId);
    }
}