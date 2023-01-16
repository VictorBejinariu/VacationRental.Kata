using System;
using System.Threading.Tasks;
using VacationRental.Application.Abstractions;
using VacationRental.Domain;

namespace VacationRental.Application.BookingHydrators
{
    public class PreparationBookingAdditionalWorkHydrator:IBookingAdditionalWorkHydrator
    {
        public string Key { get; } = "preparation";

        private readonly IPreparationRepository _preparationRepository;

        public PreparationBookingAdditionalWorkHydrator(IPreparationRepository preparationRepository)
        {
            _preparationRepository = preparationRepository??throw new ArgumentNullException(nameof(preparationRepository));
        }
        
        public async Task Hydrate(Booking booking)
        {
            var preparation = await _preparationRepository.GetByBookingId(booking.Id);
            booking.Nights += preparation.Nights;
        }
    }
}