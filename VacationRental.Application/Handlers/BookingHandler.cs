using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lodgify.Context;
using Lodgify.Dates;
using VacationRental.Application.Abstractions;
using VacationRental.Application.Contracts;
using VacationRental.Domain;
using static VacationRental.Application.Constants;

namespace VacationRental.Application.Handlers
{
    internal class BookingHandler:IBookingHandler
    {
        private readonly IBookingService _bookingService;
        private readonly IRentalService _rentalService;
        private readonly IUnitRepository _unitRepository;
        private readonly IEnumerable<IBookingActualStartHydrator> _actualStartHydrators;
        private readonly IEnumerable<IBookingActualNightsHydrator> _actualNightsHydrators;

        public BookingHandler(
            IBookingService bookingRepository,
            IRentalService rentalRepository,
            IUnitRepository unitRepository,
            IEnumerable<IBookingActualNightsHydrator> actualNightsHydrators,
            IEnumerable<IBookingActualStartHydrator> actualStartHydrators)
        {
            _bookingService = bookingRepository??throw new ArgumentNullException(nameof(bookingRepository));
            _rentalService = rentalRepository??throw new ArgumentNullException(nameof(rentalRepository));
            _unitRepository = unitRepository??throw new ArgumentNullException(nameof(unitRepository));
            _actualStartHydrators = actualStartHydrators ?? new List<IBookingActualStartHydrator>();
            _actualNightsHydrators = actualNightsHydrators??new List<IBookingActualNightsHydrator>();
        }
        
        public async Task<RequestHandler<Booking>> GetById(int bookingId)
        {
            var result = await _bookingService.GetById(bookingId);

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
                .AndThenTry(CreateBooking)
                .AndThenTry(SelectAvailableUnit)
                .AndThenTry(PersistBooking)
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
            var rental = await _rentalService.GetById(context.Request.RentalId);
            
            if (rental == null)
            {
                context.Handler.With(Error.WithMessage(RentalNotFoundErrorMessage));
            }

            context.Rental = rental;
            return context;
        }
        private async Task<BookingCreateContext> SelectAvailableUnit(BookingCreateContext context)
        {
            var newBooking = context.BookingEntity;
            
            var rentalUnits = await _unitRepository.GetByRentalId(context.Rental.Id);
            foreach (var unit in rentalUnits)
            {
                if (!await IsUnitAvailableForBooking(unit, newBooking))
                {
                    continue;
                }

                context.BookingEntity.UnitId = unit.Id;
                context.AvailableUnit = unit;
                return context;
            }
            
            context.Handler.With(Error.WithMessage(NotAvailableErrorMessage));
            return context;
        }
        private async Task<bool> IsUnitAvailableForBooking(Unit unit, Booking newBooking)
        {
            var existingBookings = await _bookingService.GetByUnitId(unit.Id);
            foreach (var booking in existingBookings)
            {
                if (await AreOverlapping(booking, newBooking))
                {
                    return false;
                }
            }
            return true;
        } 
        private async Task<bool> AreOverlapping(Booking existingBooking, Booking newBooking)
        {
            var existingBookingStart = existingBooking.Start;

            foreach (var hydrator in _actualStartHydrators)
            {
                existingBookingStart = await hydrator.Hydrate(existingBooking, existingBookingStart);
            }
            
            var existingBookingNights = existingBooking.Nights;
            
            foreach (var hydrator in _actualNightsHydrators)
            {
                existingBookingNights = await hydrator.Hydrate(existingBooking, existingBookingNights);
            }

            return new DateInterval(
                    existingBookingStart, 
                    existingBookingStart.AddDays(existingBookingNights))
                .IsOverlapping(
                    new DateInterval(
                        newBooking.Start, 
                        newBooking.Start.AddDays(newBooking.Nights)));
        }
        private Task<BookingCreateContext> CreateBooking(BookingCreateContext context)
        {
            var newBooking = new Booking()
            {
                Nights = context.Request.Nights,
                RentalId = context.Request.RentalId,
                Start = context.Request.Start.Date
            };

            context.BookingEntity = newBooking;
            
            return Task.FromResult(context);
        }
        private async Task<BookingCreateContext> PersistBooking(BookingCreateContext context)
        {
            var newBooking = context.BookingEntity;
            if (!await _bookingService.Create(newBooking))
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