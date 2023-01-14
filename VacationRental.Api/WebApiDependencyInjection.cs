using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VacationRental.Api.Models;
using VacationRental.Application.Contracts;
using VacationRental.Infrastructure.Contracts;

namespace VacationRental.Api
{
    public static class WebApiDependencyInjection
    {
        public static IServiceCollection AddVacationRentalApp(
            this IServiceCollection @this, 
            IConfiguration configuration)
        {
            @this.RegisterApplication();
            @this.RegisterInfrastructure();
            
            @this.AddSingleton<IDictionary<int, RentalViewModel>>(new Dictionary<int, RentalViewModel>());
            @this.AddSingleton<IDictionary<int, BookingViewModel>>(new Dictionary<int, BookingViewModel>());
            
            return @this;
        }
    }
}