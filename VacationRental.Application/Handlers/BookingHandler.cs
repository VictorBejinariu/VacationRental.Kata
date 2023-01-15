using System;
using System.Threading.Tasks;
using Lodgify.Context;
using VacationRental.Application.Abstractions;
using VacationRental.Application.Contracts;
using VacationRental.Domain;

namespace VacationRental.Application.Handlers
{
    internal class BookingHandler:IBookingHandler
    {
        private readonly IBookingService _bookingRepository;
        private readonly IRentalService _rentalRepository;

        public BookingHandler(
            IBookingService bookingRepository,
            IRentalService rentalRepository)
        {
            _bookingRepository = bookingRepository??throw new ArgumentNullException(nameof(bookingRepository));
            _rentalRepository = rentalRepository??throw new ArgumentNullException(nameof(rentalRepository));
        }
        
        public async Task<RequestHandler<Booking>> GetById(int bookingId)
        {
            var result = await _bookingRepository.GetById(bookingId);

            if (result == null)
            {
                return RequestHandler<Booking>.New().With(Error.WithMessage("Booking not found"));
            }

            return RequestHandler<Booking>.New().With(result);
        }

        public async Task<RequestHandler<BookingCreateResponse>> Create(BookingCreate input)
        {
            //Validation
            if (input.Nights <= 0)
            {
                return RequestHandler<BookingCreateResponse>.New()
                    .With(Error.WithMessage("Nights must be positive"));
            }

            //ReadRental
            var rental = await _rentalRepository.GetById(input.RentalId);
            
            if (rental == null)
            {
                return RequestHandler<BookingCreateResponse>.New()
                    .With(Error.WithMessage("Rental not found"));
            }

            //CheckOverlapping
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
                {
                    return RequestHandler<BookingCreateResponse>.New()
                        .With(Error.WithMessage("Not available"));
                }
                
            }
            //Create Booking
            var newBooking = new Booking()
            {
                Nights = input.Nights,
                RentalId = input.RentalId,
                Start = input.Start.Date
            };

            if (!await _bookingRepository.Create(newBooking))
            {
                return RequestHandler<BookingCreateResponse>.New().With(Error.WithMessage("Failed Insert"));
            }

            return RequestHandler<BookingCreateResponse>.New().With(new BookingCreateResponse()
            {
                BookingId = newBooking.Id
            });
        }
    }
}