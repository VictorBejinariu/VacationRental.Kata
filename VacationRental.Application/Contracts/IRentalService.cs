using Lodgify.Context;
using VacationRental.Domain;

namespace VacationRental.Application.Contracts
{
    public interface IRentalService
    {
        ApiReadingContext<Rental> GetById(int rentalId);
    }
}