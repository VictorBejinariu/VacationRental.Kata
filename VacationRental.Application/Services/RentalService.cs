using System;
using System.Threading.Tasks;
using Lodgify.Context;
using VacationRental.Application.Abstractions;
using VacationRental.Application.Contracts;
using VacationRental.Domain;
using static VacationRental.Application.Constants;

namespace VacationRental.Application.Services
{
    internal class RentalService:IRentalService
    {
        private readonly IRentalRepository _rentalRepository;

        public RentalService(IRentalRepository rentalRepository)
        {
            _rentalRepository = rentalRepository??throw new ArgumentNullException(nameof(rentalRepository));
        }
        
        public async Task<ApiRequestContext<Rental>> GetById(int rentalId)
        {
            var rental = await _rentalRepository.GetById(rentalId);
            if (rental == null)
            {
                return ApiRequestContext<Rental>
                    .New().With(Error.WithMessage(RentalNotFoundErrorMessage));
            }
            return ApiRequestContext<Rental>.New().With(rental);
        }

        public async Task<ApiRequestContext<RentalCreateResponse>> Create(RentalCreate input)
        {
            var rental = new Rental()
            {
                Units = input.Units
            };

            if (!await _rentalRepository.Create(rental))
            {
                return ApiRequestContext<RentalCreateResponse>.New().With(Error.WithMessage("Failed to Insert Rental"));
            }

            return ApiRequestContext<RentalCreateResponse>.New().With(new RentalCreateResponse() {RentalId = rental.Id});
        }
    }
}