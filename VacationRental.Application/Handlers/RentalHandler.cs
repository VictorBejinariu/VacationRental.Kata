using System;
using System.Threading.Tasks;
using Lodgify.Context;
using Lodgify.Dates;
using VacationRental.Application.Abstractions;
using VacationRental.Application.Contracts;
using VacationRental.Domain;
using static VacationRental.Application.Constants;

namespace VacationRental.Application.Handlers
{
    internal class RentalHandler:IRentalHandler
    {
        private readonly IRentalService _rentalService;
        private readonly IUnitRepository _unitRepository;
        private readonly IBookingService _bookingService;
        private readonly IPreparationRepository _preparationRepository;

        public RentalHandler(IRentalService rentalRepository,
            IUnitRepository unitRepository,
            IBookingService bookingService,
            IPreparationRepository preparationRepository)
        {
            _rentalService = rentalRepository??throw new ArgumentNullException(nameof(rentalRepository));
            _unitRepository = unitRepository??throw new ArgumentNullException(nameof(unitRepository));
            _preparationRepository = preparationRepository??throw new ArgumentNullException(nameof(preparationRepository));
            _bookingService = bookingService??throw new ArgumentNullException(nameof(bookingService));
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
                    .With(Error.WithMessage(FailedInsertErrorMessage));
            }

            return RequestHandler<RentalCreateResponse>
                .New()
                .With(new RentalCreateResponse() {RentalId = rental.Id});
        }

        public async Task<RequestHandler<RentalUpdateResponse>> Update(RentalUpdate input)
        {
            if (input.Units != null && input.Units <= 0)
            {
                return RequestHandler<RentalUpdateResponse>
                    .New()
                    .With(Error.WithMessage(UnitsMustBePositiveErrorMessage));
            }

            if (input.PreparationTimeInDays != null && input.PreparationTimeInDays <= 0)
            {
                return RequestHandler<RentalUpdateResponse>
                    .New()
                    .With(Error.WithMessage(PreparationTimeInDaysMustBePositiveErrorMessage));
            }
            
            var rentalEntity = await _rentalService.GetById(input.Id);
            if (rentalEntity is null)
            {
                return RequestHandler<RentalUpdateResponse>
                    .New()
                    .With(Error.WithMessage(RentalNotFoundErrorMessage));
            }

            rentalEntity.Units = input.Units ?? rentalEntity.Units;

            if (input.PreparationTimeInDays == null)
            {
                return RequestHandler<RentalUpdateResponse>.New().With(new RentalUpdateResponse());
            }

            var rentalUnits = await _unitRepository.GetByRentalId(input.Id);

            foreach (var unit in rentalUnits)
            {
                var preparations = await _preparationRepository.GetByUnitId(unit.Id);
                foreach (var preparation in preparations)
                {
                    var previousPreparation = preparation.Nights;
                    preparation.Nights = input.PreparationTimeInDays.Value;
                    var bookings = await _bookingService.GetByUnitId(unit.Id);
                    {
                        foreach (var booking in bookings)
                        {
                            if (AreOverlapping(booking, preparation))
                            {
                                preparation.Nights = previousPreparation;
                                return RequestHandler<RentalUpdateResponse>.New().With(Error.WithMessage(PreparationOverlappingErrorMessage));
                            }
                        }
                    }
                }
            }
            
            return RequestHandler<RentalUpdateResponse>.New().With(new RentalUpdateResponse());

        }
        private bool AreOverlapping(Booking booking, Preparation preparation)
        {
            return new DateInterval(
                    booking.Start, 
                    booking.Start.AddDays(booking.Nights))
                .IsOverlapping(
                    new DateInterval(
                        preparation.Start, 
                        preparation.Start.AddDays(preparation.Nights)));
        }
    }
}