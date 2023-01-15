using System.Threading.Tasks;
using Lodgify.Context;
using VacationRental.Domain;

namespace VacationRental.Application.Contracts
{
    public interface IBookingHandler
    {
        Task<RequestHandler<Booking>> GetById(int bookingId);
        Task<RequestHandler<BookingCreateResponse>> Create(BookingCreate input);
    }
}