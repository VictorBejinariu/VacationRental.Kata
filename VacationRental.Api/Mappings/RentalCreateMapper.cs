using VacationRental.Api.Models;
using VacationRental.Application.Contracts;

namespace VacationRental.Api.Mappings
{
    public class RentalCreateMapper
    {
        public RentalCreate From(RentalBindingModel input) =>
            new RentalCreate()
            {
                Units = input.Units,
                PreparationTimeIndDays = input.PreparationTimeInDays
            };
    }
}