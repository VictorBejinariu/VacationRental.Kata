using System;

namespace VacationRental.Application.Contracts
{
    public class BookingCreate
    {
        public int RentalId { get; set; }
        public DateTime Start { get; set; }
        public int Nights { get; set; }
    }
}