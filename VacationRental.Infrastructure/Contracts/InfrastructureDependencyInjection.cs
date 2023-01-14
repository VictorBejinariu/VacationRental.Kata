using Microsoft.Extensions.DependencyInjection;

namespace VacationRental.Infrastructure.Contracts
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection RegisterInfrastructure(this IServiceCollection @this)
        {

            return @this;
        }
    }
}