using System.Collections.Generic;
using System.Threading.Tasks;
using VacationRental.Domain;

namespace VacationRental.Application.Abstractions
{
    public interface IUnitRepository
    {
        Task<bool> Create(Unit unit);
        Task<Unit> Get(int id);
        Task<ICollection<Unit>> GetByRentalId(int rentalId);
    }
}