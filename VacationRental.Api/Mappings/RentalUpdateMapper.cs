using VacationRental.Api.Models;
using VacationRental.Application.Contracts;

namespace VacationRental.Api.Mappings
{
    public class RentalUpdateMapper
    {
        public RentalUpdate From(RentalUpdateBindingModel input) =>
            new RentalUpdate()
            {
                Units = input.Units,
                PreparationTimeInDays = input.PreparationTimeInDays
            };
    }
}