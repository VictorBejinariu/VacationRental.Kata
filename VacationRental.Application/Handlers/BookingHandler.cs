using System;
using System.Threading.Tasks;
using Lodgify.Context;
using VacationRental.Application.Abstractions;
using VacationRental.Application.Contracts;
using VacationRental.Domain;
using static VacationRental.Application.Constants;

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
                return RequestHandler<Booking>.New().With(Error.WithMessage(BookingNotFoundErrorMessage));
            }

            return RequestHandler<Booking>.New().With(result);
        }

        public async Task<RequestHandler<BookingCreateResponse>> Create(BookingCreate input) =>
            (await BookingCreateAsyncContextBuilder.From(input)
                .AndThenAlways(Validate)
                .AndThenTry(ReadCorrespondingRental)
                .AndThenTry(CheckOverlapping)
                .AndThenTry(CreateBooking)
                .Run()
            ).Handler;

        private Task<BookingCreateContext> Validate(BookingCreateContext context)
        {
            if (context.Request.Nights <= 0)
            {
                context.Handler.With(Error.WithMessage(NightsMustBePositiveErrorMessage));
            }
            return Task.FromResult(context);
        }
        private async Task<BookingCreateContext> ReadCorrespondingRental(BookingCreateContext context)
        {
            var rental = await _rentalRepository.GetById(context.Request.RentalId);
            
            if (rental == null)
            {
                context.Handler.With(Error.WithMessage(RentalNotFoundErrorMessage));
            }

            context.Rental = rental;
            return context;
        }
        private async Task<BookingCreateContext> CheckOverlapping(BookingCreateContext context)
        {

            var count = 0;
            foreach (var booking in await _bookingRepository.Get())
            {
                if (booking.RentalId == context.Request.RentalId
                    && (booking.Start <= context.Request.Start.Date && booking.Start.AddDays(booking.Nights) > context.Request.Start.Date)
                    || (booking.Start < context.Request.Start.AddDays(context.Request.Nights) && booking.Start.AddDays(booking.Nights) >= context.Request.Start.AddDays(context.Request.Nights))
                    || (booking.Start > context.Request.Start && booking.Start.AddDays(booking.Nights) < context.Request.Start.AddDays(context.Request.Nights)))
                {
                    count++;
                }
            }
            
            if (count >= context.Rental.Units)
            {
                context.Handler.With(Error.WithMessage(NotAvailableErrorMessage));
            }

            return context;
        }
        private async Task<BookingCreateContext> CreateBooking(BookingCreateContext context)
        {
            var newBooking = new Booking()
            {
                Nights = context.Request.Nights,
                RentalId = context.Request.RentalId,
                Start = context.Request.Start.Date
            };

            if (!await _bookingRepository.Create(newBooking))
            {
                context.Handler.With(Error.WithMessage(FailedInsertErrorMessage));
            }

            context.Handler.With(new BookingCreateResponse()
            {
                BookingId = newBooking.Id
            });

            return context;
        }

    }
}