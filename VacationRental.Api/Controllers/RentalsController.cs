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
        private readonly RentalUpdateMapper _rentalUpdateMapper;


        public RentalsController(
            IRentalHandler rentalHandler,
            RentalViewModelMapper rentalViewModelMapper,
            RentalCreateMapper rentalCreateMapper,
            RentalUpdateMapper rentalUpdateMapper)
        {
            _rentalHandler = rentalHandler??throw new ArgumentNullException(nameof(rentalHandler));
            _rentalViewModelMapper =
                rentalViewModelMapper ?? throw new ArgumentNullException(nameof(rentalViewModelMapper));
            _rentalCreateMapper = rentalCreateMapper??throw new ArgumentNullException(nameof(rentalCreateMapper));
            _rentalUpdateMapper = rentalUpdateMapper??throw new ArgumentNullException(nameof(rentalUpdateMapper));
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
            var response = await _rentalHandler.Create(createRequest);

            if (!response.Execution.IsSuccess)
            {
                throw new ApplicationException(response.Execution.Error.Message);
            }

            return new ResourceIdViewModel()
            {
                Id = response.Data.RentalId
            };
        }

        [HttpPut]
        [Route("{rentalId:int}")]
        public async Task<IActionResult> Put(int rentalId, RentalUpdateBindingModel model)
        {
            var request = _rentalUpdateMapper.From(model);
            request.Id = rentalId;

            var response = await _rentalHandler.Update(request);

            if (!response.Execution.IsSuccess)
            {
                throw new ApplicationException(response.Execution.Error.Message);
            }
            
            return Ok();
        } 
    }
}
