using System.Collections.Generic;
using System.Threading.Tasks;
using VacationRental.Domain;

namespace VacationRental.Application.Abstractions
{
    public interface IPreparationRepository
    {
        Task<Preparation> GetById(int preparationId);
        Task<Preparation> GetByBookingId(int bookingId);
        Task<bool> Create(Preparation preparation);
        Task<ICollection<Preparation>> Get();
        Task<ICollection<Preparation>> GetByUnitId(int unitId);
    }
}