using System.Threading.Tasks;
using Lodgify.Context;
using VacationRental.Domain;

namespace VacationRental.Application.Contracts
{
    public interface IRentalService
    {
        Task<ApiRequestContext<Rental>> GetById(int rentalId);
        Task<ApiRequestContext<RentalCreateResponse>> Create(RentalCreate input);
    }
}