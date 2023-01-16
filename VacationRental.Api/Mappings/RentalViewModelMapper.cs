using VacationRental.Api.Models;
using VacationRental.Domain;

namespace VacationRental.Api.Mappings
{
    public class RentalViewModelMapper
    {
        public RentalViewModel From(Rental rental) =>
            new RentalViewModel()
            {
                Id = rental.Id,
                Units = rental.Units,
                PreparationTimeInDays = rental.PreparationTimeInDays
            };
    }
}