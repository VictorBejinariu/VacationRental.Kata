﻿using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using VacationRental.Application.Abstractions;
using VacationRental.Application.BookingHydrators;
using VacationRental.Application.Handlers;
using VacationRental.Application.Services;

namespace VacationRental.Application.Contracts
{
    public static class ApplicationDependencyInjection
    {
        public static IServiceCollection RegisterApplication(this IServiceCollection @this)
        {
            @this.AddSingleton<IRentalHandler, RentalHandler>();
            @this.AddSingleton<IBookingHandler, BookingHandler>();
            @this.AddSingleton<IRentalService, RentalService>();
            @this.AddSingleton<IBookingService, BookingService>();
            @this.AddSingleton<ICalendarHandler, CalendarHandler>();

            @this.RegisterBookingAdditionalWorkHydrators();
            
            return @this;
        }

        private static IServiceCollection RegisterBookingAdditionalWorkHydrators(this IServiceCollection @this)
        {
            @this.AddSingleton<IBookingActualNightsHydrator, PreparationBookingActualNightsHydrator>();
            return @this;
        }
    }
}