using VacationRental.Api.Models;
using VacationRental.Domain;

namespace VacationRental.Api.Mappings
{
    public class BookingViewModelMapper
    {
        public BookingViewModel From(Booking input) =>
            new BookingViewModel()
            {
                Id = input.Id,
                Nights = input.Nights,
                Start = input.Start,
                RentalId = input.RentalId
            };
    }
}