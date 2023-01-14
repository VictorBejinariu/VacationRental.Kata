using System.Threading.Tasks;
using Lodgify.Context;
using VacationRental.Domain;

namespace VacationRental.Application.Contracts
{
    public interface IRentalService
    {
        Task<ApiReadContext<Rental>> GetById(int rentalId);
        Task<ApiCreateContext<RentalCreate, RentalCreateResponse>> Create(RentalCreate input);

    }
}