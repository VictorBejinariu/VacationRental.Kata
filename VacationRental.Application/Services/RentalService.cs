using System.Threading.Tasks;
using Lodgify.Context;
using VacationRental.Application.Contracts;
using VacationRental.Domain;

namespace VacationRental.Application.Services
{
    internal class RentalService:IRentalService
    {
        public Task<ApiReadContext<Rental>> GetById(int rentalId)
        {
            throw new System.NotImplementedException();
        }

        public Task<ApiCreateContext<RentalCreate, RentalCreateResponse>> Create(RentalCreate input)
        {
            throw new System.NotImplementedException();
        }
    }
}