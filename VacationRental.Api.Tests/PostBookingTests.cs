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
    public class PostBookingTests
    {
        private static readonly DateTime SomeStartDay = DateTime.Today;
        private static readonly DateTime SomeEndDay = DateTime.Today.AddDays(10);
        private const int SomeNights = 3;
        private const int NegativeNightsValue = -10;
        private const int IdForNonExistingRental = 91919191;
        private const string NegativeNightsErrorMessage = "Nights must be positive";
        private const string RentalsNotAvailableErrorMessage = "Not available";
        private const string RentalNotFoundErrormessage = "Rental not found";

        
        private readonly HttpClient _client;

        public PostBookingTests(IntegrationFixture fixture)
        {
            _client = fixture.Client??throw new ArgumentNullException(nameof(fixture.Client));
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenPostBooking_ThenAGetReturnsTheCreatedBooking()
        {
            var rentalResourceId = 
                await GivenIdForSuccessfulRentalPostWith(noOfUnits: 4, _client);
            
            var postBookingRequest = new BookingBindingModel
            {
                 RentalId = rentalResourceId.Id,
                 Nights = SomeNights,
                 Start = SomeStartDay
            };

            ResourceIdViewModel postBookingResult;
            using (var postBookingResponse = 
                    await _client.PostAsJsonAsync($"/api/v1/bookings", postBookingRequest))
            {
                postBookingResponse.IsSuccessStatusCode.Should().BeTrue();
                
                postBookingResult = 
                    await postBookingResponse
                        .Content
                        .ReadAsAsync<ResourceIdViewModel>();
            }

            using (var getBookingResponse = 
                    await _client.GetAsync($"/api/v1/bookings/{postBookingResult.Id}"))
            {
                getBookingResponse.IsSuccessStatusCode.Should().BeTrue();

                var getBookingResult = await getBookingResponse.Content.ReadAsAsync<BookingViewModel>();
                getBookingResult.RentalId.Should().Be(postBookingRequest.RentalId);
                getBookingResult.Nights.Should().Be(postBookingRequest.Nights);
                getBookingResult.Start.Should().Be(postBookingRequest.Start);
            }
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenPostBooking_ThenAPostReturnsErrorWhenThereIsOverbooking()
        {
            var rentalResourceId = 
                await GivenIdForSuccessfulRentalPostWith(noOfUnits: 1,_client);
            
            var postBooking1Request = new BookingBindingModel
            {
                RentalId = rentalResourceId.Id,
                Nights = 3,
                Start = new DateTime(2002, 01, 01)
            };

            using (var postBooking1Response = 
                    await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking1Request))
            {
                postBooking1Response.IsSuccessStatusCode.Should().BeTrue();
            }

            var postBooking2Request = new BookingBindingModel
            {
                RentalId = rentalResourceId.Id,
                Nights = 1,
                Start = new DateTime(2002, 01, 02)
            };

            Func<Task> act = 
                async () 
                    => await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking2Request);

            await act
                .Should()
                .ThrowAsync<ApplicationException>()
                .WithMessage(RentalsNotAvailableErrorMessage);
        }

        [Fact]
        public async Task GivenRentalWithTwoUnitsAndExistingBooking_WhenPostBookingForSamePeriod_ThenShouldNotFail()
        {
            var rentalResourceId = 
                await GivenIdForSuccessfulRentalPostWith(noOfUnits: 2,_client);
            
            var postBookingRequest = new BookingBindingModel
            {
                RentalId = rentalResourceId.Id,
                Nights = SomeNights,
                Start = SomeStartDay
            };

            ResourceIdViewModel postBookingResult;
            using (var postBookingResponse = 
                   await _client.PostAsJsonAsync($"/api/v1/bookings", postBookingRequest))
            {
                postBookingResponse.IsSuccessStatusCode.Should().BeTrue();
                await postBookingResponse
                        .Content
                        .ReadAsAsync<ResourceIdViewModel>();
            }

            Func<Task> act = async ()
                => await _client.PostAsJsonAsync($"/api/v1/bookings", postBookingRequest);

            await act.Should().NotThrowAsync<ApplicationException>();
        }
        
        [Fact]
        public async Task GivenRequestWithNegativeNightsValue_WhenPostBooking_ThenReturnsErrorWithSpecificMessage()
        {
            var rentalResourceId =
                await GivenIdForSuccessfulRentalPostWith(noOfUnits: 1, _client);

            var postBookingRequest = new BookingBindingModel
            {
                RentalId = rentalResourceId.Id,
                Nights = NegativeNightsValue,
                Start = SomeStartDay
            };

            Func<Task> act = 
                async () 
                    => await _client.PostAsJsonAsync($"/api/v1/bookings", postBookingRequest);

            await act
                .Should()
                .ThrowAsync<ApplicationException>()
                .WithMessage(NegativeNightsErrorMessage);
        }
        
        [Fact]
        public async Task GivenRequestForMissingRental_WhenPostBooking_ThenReturnsErrorWithSpecificMessage()
        {
            var postBookingRequest = new BookingBindingModel
            {
                RentalId = IdForNonExistingRental,
                Nights = SomeNights,
                Start = SomeStartDay
            };

            Func<Task> act = async () =>
            {
                using (await _client.PostAsJsonAsync($"/api/v1/bookings", postBookingRequest))
                { }
            };

            await act
                .Should()
                .ThrowAsync<ApplicationException>()
                .WithMessage(RentalNotFoundErrormessage);
        }
    }
}
