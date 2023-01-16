﻿using System;
using System.Threading.Tasks;
using Lodgify.Context;
using VacationRental.Application.Abstractions;
using VacationRental.Application.Contracts;
using VacationRental.Domain;
using static VacationRental.Application.Constants;

namespace VacationRental.Application.Handlers
{
    internal class RentalHandler:IRentalHandler
    {
        private readonly IRentalService _rentalService;

        public RentalHandler(IRentalService rentalRepository)
        {
            _rentalService = rentalRepository??throw new ArgumentNullException(nameof(rentalRepository));
        }
        
        public async Task<RequestHandler<Rental>> GetById(int rentalId)
        {
            var rental = await _rentalService.GetById(rentalId);
            if (rental == null)
            {
                return RequestHandler<Rental>
                    .New()
                    .With(Error.WithMessage(RentalNotFoundErrorMessage));
            }
            return RequestHandler<Rental>.New().With(rental);
        }

        public async Task<RequestHandler<RentalCreateResponse>> Create(RentalCreate input)
        {
            if (input.PreparationTimeIndDays.HasValue 
                && input.PreparationTimeIndDays.Value < 0)
            {
                return RequestHandler<RentalCreateResponse>
                    .New()
                    .With(Error.WithMessage(PreparationTimeInDaysMustBePositiveErrorMessage));
            }
            
            var rental = new Rental()
            {
                Units = input.Units,
                PreparationTimeInDays = input.PreparationTimeIndDays??0
            };

            if (!await _rentalService.Create(rental))
            {
                return RequestHandler<RentalCreateResponse>
                    .New()
                    .With(Error.WithMessage("Failed to Insert Rental"));
            }

            return RequestHandler<RentalCreateResponse>
                .New()
                .With(new RentalCreateResponse() {RentalId = rental.Id});
        }
    }
}