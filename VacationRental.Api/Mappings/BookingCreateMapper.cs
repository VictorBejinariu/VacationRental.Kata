using VacationRental.Api.Models;
using VacationRental.Application.Contracts;

namespace VacationRental.Api.Mappings
{
    public class BookingCreateMapper
    {
        public BookingCreate From(BookingBindingModel input) =>
            new BookingCreate()
            {
                RentalId = input.RentalId,
                Start = input.Start,
                Nights = input.Nights
            };
    }
}