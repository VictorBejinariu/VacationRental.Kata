using Microsoft.Extensions.DependencyInjection;

namespace VacationRental.Application.Contracts
{
    public static class ApplicationDependencyInjection
    {
        public static IServiceCollection RegisterApplication(this IServiceCollection @this)
        {

            return @this;
        }
    }
}