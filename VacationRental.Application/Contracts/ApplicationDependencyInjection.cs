using Microsoft.Extensions.DependencyInjection;
using VacationRental.Application.Services;

namespace VacationRental.Application.Contracts
{
    public static class ApplicationDependencyInjection
    {
        public static IServiceCollection RegisterApplication(this IServiceCollection @this)
        {
            @this.AddSingleton<IRentalService, RentalService>();
            @this.AddSingleton<IBookingService, BookingService>();
            return @this;
        }
    }
}