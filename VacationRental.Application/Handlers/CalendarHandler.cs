using System;
using System.Threading.Tasks;
using Lodgify.Context;
using VacationRental.Application.Abstractions;
using VacationRental.Application.Contracts;
using static VacationRental.Application.Constants;

namespace VacationRental.Application.Handlers
{
    internal class CalendarHandler:ICalendarHandler
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IRentalRepository _rentalRepository;

        public CalendarHandler(IBookingRepository bookingRepository, IRentalRepository rentalRepository)
        {
            _bookingRepository = bookingRepository??throw new ArgumentNullException(nameof(bookingRepository));
            _rentalRepository = rentalRepository??throw new ArgumentNullException(nameof(rentalRepository));
        }
        
        public async Task<RequestHandler<RentalCalendar>> GetRentalCalendarByInterval(int rentalId, DateTime start, int nights)
        {
            if (nights < 0)
                return RequestHandler<RentalCalendar>.New()
                    .With(Error.WithMessage(NightsMustBePositiveErrorMessage));
            
            var rental = await _rentalRepository.GetById(rentalId);
            if (rental == null)
                return RequestHandler<RentalCalendar>.New().With(Error.WithMessage(RentalNotFoundErrorMessage));
            
            return RequestHandler<RentalCalendar>.New().With(Error.WithMessage("Not implmented"));
            
        }
    }
}