using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Mappings;
using VacationRental.Api.Models;
using VacationRental.Application.Contracts;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/bookings")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly BookingViewModelMapper _bookingViewModelMapper;
        private readonly BookingCreateMapper _bookingCreateMapper;

        public BookingsController(IBookingService bookingService,
            BookingViewModelMapper bookingViewModelMapper,
            BookingCreateMapper bookingCreateMapper)
        {
            _bookingService = bookingService??throw new ArgumentNullException(nameof(bookingService));
            _bookingViewModelMapper = bookingViewModelMapper??throw new ArgumentNullException(nameof(bookingViewModelMapper));
            _bookingCreateMapper = bookingCreateMapper??throw new ArgumentNullException(nameof(bookingCreateMapper));
        }

        [HttpGet]
        [Route("{bookingId:int}")]
        public async Task<BookingViewModel> Get(int bookingId)
        {
            var get = await _bookingService.GetById(bookingId);
            if (!get.Execution.IsSuccess)
            {
                throw new ApplicationException(get.Execution.Error.Message);
            }
            return _bookingViewModelMapper.From(get.Data);
        }

        [HttpPost]
        public async Task<ResourceIdViewModel> Post(BookingBindingModel model)
        {
            var create = await _bookingService.Create(_bookingCreateMapper.From(model));

            if (!create.Execution.IsSuccess)
            {
                throw new ApplicationException(create.Execution.Error.Message);
            }

            return new ResourceIdViewModel()
            {
                Id = create.Data.BookingId
            };
        }
    }
}
