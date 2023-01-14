using System;
using System.Threading.Tasks;
using Lodgify.Context;
using VacationRental.Application.Abstractions;
using VacationRental.Application.Contracts;
using VacationRental.Domain;

namespace VacationRental.Application.Services
{
    public class BookingService:IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IRentalRepository _rentalRepository;

        public BookingService(
            IBookingRepository bookingRepository,
            IRentalRepository rentalRepository)
        {
            _bookingRepository = bookingRepository??throw new ArgumentNullException(nameof(bookingRepository));
            _rentalRepository = rentalRepository??throw new ArgumentNullException(nameof(rentalRepository));
        }
        
        public async Task<ApiRequestContext<Booking>> GetById(int bookingId)
        {
            var result = await _bookingRepository.GetById(bookingId);

            if (result == null)
            {
                return ApiRequestContext<Booking>.New().With(Error.WithMessage("Booking not found"));
            }

            return ApiRequestContext<Booking>.New().With(result);
        }

        public async Task<ApiRequestContext<BookingCreateResponse>> Create(BookingCreate input)
        {
            if (input.Nights <= 0)
            {
                throw new ApplicationException("Nigts must be positive");
            }

            var rental = await _rentalRepository.GetById(input.RentalId);
            
            if (rental == null)
            {
                throw new ApplicationException("Rental not found");
            }

            for (var i = 0; i < input.Nights; i++)
            {
                var count = 0;
                foreach (var booking in await _bookingRepository.Get())
                {
                    if (booking.RentalId == input.RentalId
                        && (booking.Start <= input.Start.Date && booking.Start.AddDays(booking.Nights) > input.Start.Date)
                        || (booking.Start < input.Start.AddDays(input.Nights) && booking.Start.AddDays(booking.Nights) >= input.Start.AddDays(input.Nights))
                        || (booking.Start > input.Start && booking.Start.AddDays(booking.Nights) < input.Start.AddDays(input.Nights)))
                    {
                        count++;
                    }
                }
                if (count >= rental.Units)
                    throw new ApplicationException("Not available");
            }

            var newBooking = new Booking()
            {
                Nights = input.Nights,
                RentalId = input.RentalId,
                Start = input.Start.Date
            };

            if (!await _bookingRepository.Create(newBooking))
            {
                return ApiRequestContext<BookingCreateResponse>.New().With(Error.WithMessage("Failed Insert"));
            }

            return ApiRequestContext<BookingCreateResponse>.New().With(new BookingCreateResponse()
            {
                BookingId = newBooking.Id
            });
        }
    }
}