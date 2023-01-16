using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VacationRental.Application.Abstractions;
using VacationRental.Domain;

namespace VacationRental.Application.Services
{
    internal class BookingService:IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IRentalService _rentalService;
        private readonly IPreparationRepository _preparationRepository;

        public BookingService(IBookingRepository bookingRepository, 
            IRentalService rentalService,
            IPreparationRepository preparationRepository)
        {
            _bookingRepository = bookingRepository??throw new ArgumentNullException(nameof(bookingRepository));
            _rentalService = rentalService??throw new ArgumentNullException(nameof(rentalService));
            _preparationRepository =
                preparationRepository ?? throw new ArgumentNullException(nameof(preparationRepository));
        }
        public async Task<ICollection<Booking>> Get()
        {
            return await _bookingRepository.Get();
        }

        public Task<Booking> GetById(int bookingId)
        {
            return _bookingRepository.GetById(bookingId);
        }

        public async Task<bool> Create(Booking input)
        {
            if (!await _bookingRepository.Create(input))
            {
                return false;
            }
            
            //TODO: Try Raise event of BookingCreated
            
            var rental = await _rentalService.GetById(input.RentalId);

            await _preparationRepository.Create(new Preparation()
            {
                BookingId = input.Id,
                RentalId = input.RentalId,
                UnitId = input.UnitId,
                Start = input.Start.AddDays(input.Nights).AddDays(1),
                Nights = rental.PreparationTimeInDays
            });
            
            return true;
        }

        public async Task<ICollection<Booking>> GetByUnitId(int unitId)
        {
            return await _bookingRepository.GetByUnitId(unitId);
        }
    }
}