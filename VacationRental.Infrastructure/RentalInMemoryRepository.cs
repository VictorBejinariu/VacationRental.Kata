using System.Collections.Generic;
using System.Threading.Tasks;
using VacationRental.Application.Abstractions;
using VacationRental.Domain;

namespace VacationRental.Infrastructure
{
    internal class RentalInMemoryRepository:IRentalRepository
    {
        private const int FailedInsertResult = -1;
        private readonly Dictionary<int, Rental> _data = new Dictionary<int, Rental>();

        public Task<Rental> GetById(int rentalId)
        {
            if (!_data.ContainsKey(rentalId))
            {
                return Task.FromResult<Rental>(null);
            }
            return Task.FromResult(_data[rentalId]);
        }

        public Task<bool> Create(Rental input)
        {
            input.Id = _data.Keys.Count + 1;

            try
            {
                _data.Add(input.Id, input);
            }
            catch
            {
                input.Id = FailedInsertResult;
            }

            return Task.FromResult(input.Id>0);
        }
    }
}