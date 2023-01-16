using Lodgify.Context;
using VacationRental.Application.Contracts;
using VacationRental.Domain;

namespace VacationRental.Application.Handlers
{
    public class BookingCreateContext:IContext<BookingCreateResponse>
    {
        protected BookingCreateContext()
        {
            
        }
        
        public BookingCreate Request { get; set; }
        public RequestHandler<BookingCreateResponse> Handler { get; set; }
        public Rental Rental{ get; set; }
        public Unit AvailableUnit { get; set; }

        public static BookingCreateContext From(BookingCreate request)
        {
            var result = new BookingCreateContext()
            {
                Request = request,
                Handler = RequestHandler<BookingCreateResponse>.New()
            };
            return result;
        }
    }
}