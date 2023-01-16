using System;
using System.Linq;
using System.Threading.Tasks;
using VacationRental.Application.Abstractions;
using VacationRental.Domain;

namespace VacationRental.Application.Services
{
    internal class RentalService:IRentalService
    {
        private readonly IRentalRepository _rentalRepository;
        private readonly IUnitRepository _unitRepository;

        public RentalService(IRentalRepository rentalRepository, IUnitRepository unitRepository)
        {
            _rentalRepository = rentalRepository??throw new ArgumentNullException(nameof(rentalRepository));
            _unitRepository = unitRepository??throw new ArgumentNullException(nameof(unitRepository));
        }
        public async Task<Rental> GetById(int rentalId)
        {
            return await _rentalRepository.GetById(rentalId);
        }

        public async Task<bool> Create(Rental input)
        {
            if (!await _rentalRepository.Create(input))
            {
                return false;
            }
            
            //TODO:Try later to raise event RentalCreated
            
            foreach (var unitId in Enumerable.Range(1,input.Units))
            {
                var newUnit = new Unit()
                {
                    RentalId = input.Id,
                    RentalUnitId = unitId
                };

                await _unitRepository.Create(newUnit);
                //TODO: If this fails: Revert changes if flow stays sync / Raise Event if Event Driven
            }
            
            return true;
        }
    }
}