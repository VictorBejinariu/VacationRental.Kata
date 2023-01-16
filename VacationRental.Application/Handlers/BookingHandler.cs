using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IBookingService _bookingService;
        private readonly IRentalService _rentalService;
        private readonly IUnitRepository _unitRepository;
        private readonly IEnumerable<IBookingAdditionalWorkHydrator> _additionalWorkHydrators;

        public BookingHandler(
            IBookingService bookingRepository,
            IRentalService rentalRepository,
            IUnitRepository unitRepository,
            IEnumerable<IBookingAdditionalWorkHydrator> hydrators)
        {
            _bookingService = bookingRepository??throw new ArgumentNullException(nameof(bookingRepository));
            _rentalService = rentalRepository??throw new ArgumentNullException(nameof(rentalRepository));
            _unitRepository = unitRepository??throw new ArgumentNullException(nameof(unitRepository));
            _additionalWorkHydrators = hydrators??new List<IBookingAdditionalWorkHydrator>();
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
                .AndThenTry(SelectAvailableUnit)
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
            var newBooking = context.Request;
            
            var rentalUnits = await _unitRepository.GetByRentalId(context.Rental.Id);
            foreach (var unit in rentalUnits)
            {
                if (!await IsUnitAvailableForBooking(unit, newBooking))
                {
                    continue;
                }
                context.AvailableUnit = unit;
                return context;
            }
            
            context.Handler.With(Error.WithMessage(NotAvailableErrorMessage));
            return context;
        }
        private async Task<bool> IsUnitAvailableForBooking(Unit unit, BookingCreate newBooking)
        {
            var existingBookings = await _bookingService.GetByUnitId(unit.Id);
            foreach (var booking in existingBookings)
            {
                foreach (var hydrator in _additionalWorkHydrators)
                {
                    await hydrator.Hydrate(booking);
                }
            }
            return !existingBookings.Any(b=>CheckOverlapping(b,newBooking));

        } 
        private bool CheckOverlapping(Booking existingBooking, BookingCreate newBooking)
        {
            return existingBooking.Start <= newBooking.Start.Date
                        && existingBooking.Start.AddDays(existingBooking.Nights) > newBooking.Start.Date
                    || existingBooking.Start < newBooking.Start.AddDays(newBooking.Nights)
                        && existingBooking.Start.AddDays(existingBooking.Nights) >= newBooking.Start.AddDays(newBooking.Nights)
                    || existingBooking.Start > newBooking.Start
                        && existingBooking.Start.AddDays(existingBooking.Nights) < newBooking.Start.AddDays(newBooking.Nights);
        }
        private async Task<BookingCreateContext> CreateBooking(BookingCreateContext context)
        {
            var newBooking = new Booking()
            {
                Nights = context.Request.Nights,
                RentalId = context.Request.RentalId,
                UnitId = context.AvailableUnit.Id,
                Start = context.Request.Start.Date
            };

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