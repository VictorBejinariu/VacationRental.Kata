namespace VacationRental.Application.Contracts
{
    public class RentalUpdate
    {
        public int Id { get; set; }
        public int? Units { get; set; }
        public int? PreparationTimeInDays { get; set; }
    }
}