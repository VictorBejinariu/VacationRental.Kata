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
        private readonly IRentalService _rentalService;
        private readonly RentalViewModelMapper _rentalViewModelMapper;
        private readonly RentalCreateMapper _rentalCreateMapper;

        public RentalsController(
            IRentalService rentalService,
            RentalViewModelMapper rentalViewModelMapper,
            RentalCreateMapper rentalCreateMapper)
        {
            _rentalService = rentalService??throw new ArgumentNullException(nameof(rentalService));
            _rentalViewModelMapper =
                rentalViewModelMapper ?? throw new ArgumentNullException(nameof(rentalViewModelMapper));
            _rentalCreateMapper = rentalCreateMapper??throw new ArgumentNullException(nameof(rentalCreateMapper));
        }

        [HttpGet]
        [Route("{rentalId:int}")]
        public async Task<RentalViewModel> Get(int rentalId)
        {
            var get = await _rentalService.GetById(rentalId);

            if (!get.Execution.IsSuccess)
            {
                throw new ApplicationException(get.Execution.Error.Message);
            }

            return _rentalViewModelMapper.From(get.Data);
        }

        [HttpPost]
        public async Task<ResourceIdViewModel> Post(RentalBindingModel model)
        {
            var createRequest = _rentalCreateMapper.From(model);
            var create = await _rentalService.Create(createRequest);

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
