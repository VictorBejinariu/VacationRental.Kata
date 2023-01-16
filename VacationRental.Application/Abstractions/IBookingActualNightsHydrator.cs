using System.Threading.Tasks;
using VacationRental.Domain;

namespace VacationRental.Application.Abstractions
{
    public interface IBookingActualNightsHydrator
    {
        string Key { get;}
        Task<int> Hydrate(Booking booking, int previousValue);
    }
}