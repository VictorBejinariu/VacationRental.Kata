using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using VacationRental.Api.Models;
using Xunit;
using static VacationRental.Api.Tests.Utils;

namespace VacationRental.Api.Tests
{
    [Collection("Integration")]
    public class GetCalendarTests
    {
        private const int NegativeValueOfNights = -3;
        private const int SomeNights = 3;
        private const int SomeNonExistentRentalId = 239271237;
        
        private readonly HttpClient _client;

        public GetCalendarTests(IntegrationFixture fixture)
        {
            _client = fixture.Client??throw new ArgumentNullException(nameof(fixture.Client));
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenGetCalendar_ThenAGetReturnsTheCalculatedCalendar()
        {
            var postRentalResult = 
                GivenIdForSuccessfulRentalPostWith(2, _client);

            var postBooking1Request = new BookingBindingModel
            {
                 RentalId = postRentalResult.Id,
                 Nights = 2,
                 Start = new DateTime(2000, 01, 02)
            };

            ResourceIdViewModel postBooking1Result;
            using (var postBooking1Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking1Request))
            {
                Assert.True(postBooking1Response.IsSuccessStatusCode);
                postBooking1Result = await postBooking1Response.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            var postBooking2Request = new BookingBindingModel
            {
                RentalId = postRentalResult.Id,
                Nights = 2,
                Start = new DateTime(2000, 01, 03)
            };

            ResourceIdViewModel postBooking2Result;
            using (var postBooking2Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking2Request))
            {
                postBooking2Response.IsSuccessStatusCode.Should().BeTrue();
                postBooking2Result = await postBooking2Response.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            using (var getCalendarResponse = await _client.GetAsync($"/api/v1/calendar?rentalId={postRentalResult.Id}&start=2000-01-01&nights=5"))
            {
                getCalendarResponse.IsSuccessStatusCode.Should().BeTrue();

                var getCalendarResult = await getCalendarResponse.Content.ReadAsAsync<CalendarViewModel>();
                
                getCalendarResult.RentalId
                    .Should()
                    .Be(postRentalResult.Id);
                getCalendarResult.Dates.Count
                    .Should()
                    .Be(5);
                getCalendarResult.Dates[0].Date
                    .Should()
                    .Be(new DateTime(2000, 01, 01));
                getCalendarResult.Dates[0].Bookings
                    .Should()
                    .BeEmpty();
                
                getCalendarResult.Dates[1].Date
                    .Should()
                    .Be(new DateTime(2000, 01, 02));
                getCalendarResult.Dates[1].Bookings
                    .Should()
                    .ContainSingle();
                getCalendarResult.Dates[1].Bookings
                    .Should()
                    .Contain(x => x.Id == postBooking1Result.Id);
                
                getCalendarResult.Dates[2].Date
                    .Should()
                    .Be(new DateTime(2000, 01, 03));
                getCalendarResult.Dates[2].Bookings.Count
                    .Should()
                    .Be(2);
                getCalendarResult.Dates[2].Bookings
                    .Should()
                    .Contain(b => b.Id == postBooking1Result.Id);
                getCalendarResult.Dates[2].Bookings
                    .Should()
                    .Contain(b => b.Id == postBooking1Result.Id);
                
                getCalendarResult.Dates[3].Date
                    .Should()
                    .Be(new DateTime(2000, 01, 04));
                getCalendarResult.Dates[3].Bookings
                    .Should()
                    .ContainSingle();
                getCalendarResult.Dates[3].Bookings
                    .Should()
                    .Contain(x => x.Id == postBooking2Result.Id);

                getCalendarResult.Dates[4].Date
                    .Should()
                    .Be(new DateTime(2000, 01, 05));
                getCalendarResult.Dates[4].Bookings
                    .Should()
                    .BeEmpty();
            }
        }

        [Fact]
        public async Task GivenRequestWithNegativeNights_WhenGetCalendar_ThenReturnsErrorWithSpecificMessage()
        {
            var postRentalResult = 
                GivenIdForSuccessfulRentalPostWith(2, _client);
            
            Func<Task> act = 
                async () 
                    => await _client.GetAsync(
                    $"/api/v1/calendar?rentalId={postRentalResult.Id}&start=2000-01-01&nights={NegativeValueOfNights}");

            await act.Should().ThrowAsync<ApplicationException>().WithMessage("Nights must be positive");
        }
        
        [Fact]
        public async Task GivenRequestWithNonExistingRental_WhenGetCalendar_ThenReturnsErrorWithSpecificMessage()
        {
            Func<Task> act = 
                async () 
                    => await _client.GetAsync(
                        $"/api/v1/calendar?rentalId={SomeNonExistentRentalId}&start=2000-01-01&nights={SomeNights}");

            await act.Should().ThrowAsync<ApplicationException>().WithMessage("Rental not found");
        }
        
    }
}
