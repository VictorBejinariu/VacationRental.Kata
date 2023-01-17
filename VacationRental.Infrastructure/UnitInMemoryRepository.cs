using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationRental.Application.Abstractions;
using VacationRental.Domain;

namespace VacationRental.Infrastructure
{
    public class UnitInMemoryRepository:IUnitRepository
    {
        private const int FailedInsertResult = -1;
        private readonly Dictionary<int, Unit> _data = new Dictionary<int, Unit>();
        
        public Task<bool> Create(Unit unit)
        {
            unit.Id = _data.Keys.Count + 1;

            try
            {
                _data.Add(unit.Id, unit);
            }
            catch
            {
                unit.Id = FailedInsertResult;
            }

            return Task.FromResult(unit.Id>0);
        }

        public Task<Unit> GetById(int id)
        {
            if (!_data.ContainsKey(id))
            {
                return Task.FromResult<Unit>(null);
            }
            return Task.FromResult(_data[id]);
        }

        public Task<ICollection<Unit>> GetByRentalId(int rentalId)
        {
            return Task.FromResult<ICollection<Unit>>(_data.Values.Where(u => u.RentalId == rentalId).ToList());
        }
    }
}