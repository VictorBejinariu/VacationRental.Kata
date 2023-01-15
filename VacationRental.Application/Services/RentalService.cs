using System;
using System.Threading.Tasks;
using VacationRental.Application.Abstractions;
using VacationRental.Domain;

namespace VacationRental.Application.Services
{
    internal class RentalService:IRentalService
    {
        private readonly IRentalRepository _rentalRepository;

        public RentalService(IRentalRepository rentalRepository)
        {
            _rentalRepository = rentalRepository??throw new ArgumentNullException(nameof(rentalRepository));
        }
        public Task<Rental> GetById(int rentalId)
        {
            return _rentalRepository.GetById(rentalId);
        }

        public Task<bool> Create(Rental input)
        {
            return _rentalRepository.Create(input);
        }
    }
}