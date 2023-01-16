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

        public BookingService(IBookingRepository bookingRepository, IRentalService rentalService)
        {
            _bookingRepository = bookingRepository??throw new ArgumentNullException(nameof(bookingRepository));
            _rentalService = rentalService??throw new ArgumentNullException(nameof(rentalService));
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

            return true;

        }

        public async Task<ICollection<Booking>> GetByUnitId(int unitId)
        {
            return await _bookingRepository.GetByUnitId(unitId);
        }
    }
}