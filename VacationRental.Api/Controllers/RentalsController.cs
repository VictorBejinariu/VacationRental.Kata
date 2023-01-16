using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Mappings;
using VacationRental.Api.Models;
using VacationRental.Application.Contracts;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/rentals")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private readonly IRentalHandler _rentalHandler;
        private readonly RentalViewModelMapper _rentalViewModelMapper;
        private readonly RentalCreateMapper _rentalCreateMapper;

        public RentalsController(
            IRentalHandler rentalHandler,
            RentalViewModelMapper rentalViewModelMapper,
            RentalCreateMapper rentalCreateMapper)
        {
            _rentalHandler = rentalHandler??throw new ArgumentNullException(nameof(rentalHandler));
            _rentalViewModelMapper =
                rentalViewModelMapper ?? throw new ArgumentNullException(nameof(rentalViewModelMapper));
            _rentalCreateMapper = rentalCreateMapper??throw new ArgumentNullException(nameof(rentalCreateMapper));
        }

        [HttpGet]
        [Route("{rentalId:int}")]
        public async Task<RentalViewModel> Get(int rentalId)
        {
            var rentalResult = await _rentalHandler.GetById(rentalId);

            if (!rentalResult.Execution.IsSuccess)
            {
                throw new ApplicationException(rentalResult.Execution.Error.Message);
            }

            return _rentalViewModelMapper.From(rentalResult.Data);
        }

        [HttpPost]
        public async Task<ResourceIdViewModel> Post(RentalBindingModel model)
        {
            var createRequest = _rentalCreateMapper.From(model);
            var create = await _rentalHandler.Create(createRequest);

            if (!create.Execution.IsSuccess)
            {
                throw new ApplicationException(create.Execution.Error.Message);
            }

            return new ResourceIdViewModel()
            {
                Id = create.Data.RentalId
            };
        }
    }
}
