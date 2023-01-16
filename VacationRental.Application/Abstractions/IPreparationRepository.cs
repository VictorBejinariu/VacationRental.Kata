using System.Threading.Tasks;
using VacationRental.Domain;

namespace VacationRental.Application.Abstractions
{
    public interface IPreparationRepository
    {
        Task<Preparation> GetById(int preparationId);
        Task<Preparation> GetByBookingId(int bookingId);
        Task<bool> Create(Preparation preparation);
    }
}