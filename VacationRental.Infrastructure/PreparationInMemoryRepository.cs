using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationRental.Application.Abstractions;
using VacationRental.Domain;

namespace VacationRental.Infrastructure
{
    public class PreparationInMemoryRepository:IPreparationRepository
    {
        private const int FailedInsertResult = -1;
        private readonly Dictionary<int, Preparation> _data = new Dictionary<int, Preparation>();

        public Task<Preparation> GetById(int preparationId)
        {
            if (!_data.ContainsKey(preparationId))
            {
                return Task.FromResult<Preparation>(null);
            }

            return Task.FromResult(_data[preparationId]);
        }

        public Task<Preparation> GetByBookingId(int bookingId)
        {
            return Task.FromResult(_data.Values.FirstOrDefault(p => p.BookingId == bookingId));
        }

        public Task<bool> Create(Preparation preparation)
        {
            preparation.Id = _data.Keys.Count + 1;

            try
            {
                _data.Add(preparation.Id, preparation);
            }
            catch
            {
                preparation.Id = FailedInsertResult;
            }

            return Task.FromResult(preparation.Id > 0);
        }
    }
}