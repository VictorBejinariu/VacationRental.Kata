using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Models;
using VacationRental.Application.Abstractions;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/calendar")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly IRentalRepository _rentalRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IPreparationRepository _preparationRepository;

        public CalendarController(
            IRentalRepository rentalRepository,
            IBookingRepository bookingRepository,
            IUnitRepository unitRepository,
            IPreparationRepository preparationRepository)
        {
            _rentalRepository = rentalRepository??throw new ArgumentNullException(nameof(rentalRepository));
            _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
            _unitRepository = unitRepository??throw new ArgumentNullException(nameof(unitRepository));
            _preparationRepository =
                preparationRepository ?? throw new ArgumentNullException(nameof(preparationRepository));
        }

        [HttpGet]
        public async Task<CalendarViewModel> Get(int rentalId, DateTime start, int nights)
        {
            if (nights < 0)
                throw new ApplicationException("Nights must be positive");
            var rental = await _rentalRepository.GetById(rentalId);
            if (rental ==null)
                throw new ApplicationException("Rental not found");

            var result = new CalendarViewModel 
            {
                RentalId = rentalId,
                Dates = new List<CalendarDateViewModel>() 
            };
            for (var i = 0; i < nights; i++)
            {
                if (i == 2)
                {
                    //DoSomething
                }
                var date = new CalendarDateViewModel
                {
                    Date = start.Date.AddDays(i),
                    Bookings = new List<CalendarBookingViewModel>(),
                    PreparationTimes = new List<CalendarPreparationViewModel>()
                };

                foreach (var booking in await _bookingRepository.Get())
                {
                    if (booking.RentalId == rentalId
                        && booking.Start <= date.Date && date.Date< booking.Start.AddDays(booking.Nights))
                    {
                        
                        date.Bookings.Add(new CalendarBookingViewModel
                        {
                            Id = booking.Id,
                            UnitId = (await _unitRepository.GetById(booking.UnitId)).RentalUnitId
                        });
                    }
                }

                foreach (var preparation in await _preparationRepository.Get())
                {
                    if (preparation.RentalId == rentalId
                        && preparation.Start <= date.Date && date.Date <preparation.Start.AddDays(preparation.Nights) )
                    {
                        date.PreparationTimes.Add(new CalendarPreparationViewModel()
                        {
                            UnitId = (await _unitRepository.GetById(preparation.UnitId)).RentalUnitId
                        });
                    }
                }

                result.Dates.Add(date);
            }

            return result;
        }
    }
}
