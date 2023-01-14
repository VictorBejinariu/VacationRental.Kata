using System.Threading.Tasks;
using Lodgify.Context;
using VacationRental.Domain;

namespace VacationRental.Application.Contracts
{
    public interface IBookingService
    {
        Task<ApiRequestContext<Booking>> GetById(int bookingId);
        Task<ApiRequestContext<BookingCreateResponse>> Create(BookingCreate input);
    }
}