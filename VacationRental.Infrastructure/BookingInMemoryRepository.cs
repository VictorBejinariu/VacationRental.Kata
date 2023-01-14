using System.Collections.Generic;
using System.Threading.Tasks;
using VacationRental.Application.Abstractions;
using VacationRental.Domain;

namespace VacationRental.Infrastructure
{
    internal class BookingInMemoryRepository:IBookingRepository
    {
        private const int FailedInsertResult = -1;
        
        private readonly Dictionary<int, Booking> _data = new Dictionary<int, Booking>();

        public Task<ICollection<Booking>> Get()
        {
            ICollection<Booking> result = new List<Booking>(_data.Values);
            return Task.FromResult(result);
        }

        public Task<Booking> GetById(int bookingId)
        {
            if (!_data.ContainsKey(bookingId))
            {
                return Task.FromResult<Booking>(null);
            }

            return Task.FromResult(_data[bookingId]);
        }

        public Task<bool> Create(Booking input)
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