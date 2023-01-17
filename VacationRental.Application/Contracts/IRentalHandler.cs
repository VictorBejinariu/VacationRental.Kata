using System.Threading.Tasks;
using Lodgify.Context;
using VacationRental.Domain;

namespace VacationRental.Application.Contracts
{
    public interface IRentalHandler
    {
        Task<RequestHandler<Rental>> GetById(int rentalId);
        Task<RequestHandler<RentalCreateResponse>> Create(RentalCreate input);
        Task<RequestHandler<RentalUpdateResponse>> Update(RentalUpdate input);
    }
}