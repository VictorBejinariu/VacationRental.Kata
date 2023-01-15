using System.Threading.Tasks;
using VacationRental.Domain;

namespace VacationRental.Application.Abstractions
{
    public interface IRentalService
    {
        Task<Rental> GetById(int rentalId);
        Task<bool> Create(Rental input);
    }
}