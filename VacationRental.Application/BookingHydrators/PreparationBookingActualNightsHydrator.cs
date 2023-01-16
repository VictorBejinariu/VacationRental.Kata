using System;
using System.Threading.Tasks;
using VacationRental.Application.Abstractions;
using VacationRental.Domain;

namespace VacationRental.Application.BookingHydrators
{
    public class PreparationBookingActualNightsHydrator:IBookingActualNightsHydrator
    {
        public string Key { get; } = "preparation";

        private readonly IPreparationRepository _preparationRepository;

        public PreparationBookingActualNightsHydrator(IPreparationRepository preparationRepository)
        {
            _preparationRepository = preparationRepository??throw new ArgumentNullException(nameof(preparationRepository));
        }
        
        public async Task<int> Hydrate(Booking booking, int previousValue)
        {
            var preparation = await _preparationRepository.GetByBookingId(booking.Id);
            if (preparation is null)
            {
                return previousValue;
            }
            return previousValue + preparation.Nights;
            
        }
    }
}