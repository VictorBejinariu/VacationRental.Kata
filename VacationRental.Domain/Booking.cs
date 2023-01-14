using System;

namespace VacationRental.Domain
{
    public class Booking
    {
        public long Id { get; set; }
        public int RentalId { get; set; }
        public DateTime Start { get; set; }
        public int Nights { get; set; }
    }
}