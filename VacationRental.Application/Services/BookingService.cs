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

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository??throw new ArgumentNullException(nameof(bookingRepository));
        }
        public Task<ICollection<Booking>> Get()
        {
            return _bookingRepository.Get();
        }

        public Task<Booking> GetById(int bookingId)
        {
            return _bookingRepository.GetById(bookingId);
        }

        public Task<bool> Create(Booking input)
        {
            return _bookingRepository.Create(input);
        }
    }
}