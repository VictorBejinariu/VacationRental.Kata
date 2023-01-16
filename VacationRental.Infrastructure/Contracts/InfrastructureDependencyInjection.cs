using Microsoft.Extensions.DependencyInjection;
using VacationRental.Application.Abstractions;

namespace VacationRental.Infrastructure.Contracts
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection RegisterInfrastructure(this IServiceCollection @this)
        {
            @this.AddSingleton<IRentalRepository, RentalInMemoryRepository>();
            @this.AddSingleton<IBookingRepository, BookingInMemoryRepository>();
            @this.AddSingleton<IUnitRepository, UnitInMemoryRepository>();
            return @this;
        }
    }
}