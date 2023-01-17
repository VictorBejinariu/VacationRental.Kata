using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace VacationRental.Api.Tests
{
    [Collection("Integration")]
    public class GetBookingsTests
    {
        private const string BookingNotFoundErrorMessage = "Booking not found";
        private const int IdForNonExistingBooking = 912834;

        private readonly HttpClient _client;

        public GetBookingsTests(IntegrationFixture fixture)
        {
            _client = fixture.Client??throw new ArgumentNullException(nameof(fixture.Client));
        }
        
        [Fact]
        public async Task GivenNonExistingBooking_WhenGet_ThenReturnErrorWithSpecificMessage()
        {
            Func<Task> act = 
                async () 
                    => await _client.GetAsync($"/api/v1/bookings/{IdForNonExistingBooking}");

            await act
                .Should()
                .ThrowAsync<ApplicationException>()
                .WithMessage(BookingNotFoundErrorMessage);
        }
        
    }
}