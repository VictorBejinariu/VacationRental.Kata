using System.Threading.Tasks;
using VacationRental.Domain;

namespace VacationRental.Application.Abstractions
{
    public interface IBookingAdditionalWorkHydrator
    {
        string Key { get;}
        Task Hydrate(Booking booking);
    }
}