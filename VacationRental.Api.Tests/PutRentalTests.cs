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
    public class PutRentalTests
    {
        private const int SomeRentalId = 9999923;
        private const int NewRentalUnitsValue = 5;
        private const int NewRentalPreparationDaysValue = 10;
        
        private readonly HttpClient _client;
        
        public PutRentalTests(IntegrationFixture fixture)
        {
            _client = fixture.Client??throw new ArgumentNullException(nameof(fixture.Client));
        }

        [Fact]
        public async Task GivenRequestForNonExistingRental_WhenPut_ThenFailWithSpecificMessage()
        {
            var request = new RentalUpdateBindingModel()
            {
                Units = 10,
                PreparationTimeInDays = 2
            };

            Func<Task> act =
                async () => await _client.PutAsJsonAsync($"/api/v1/rentals/{9234}", request);

            await act.Should().ThrowAsync<ApplicationException>()
                .WithMessage("Rental Not Found");
        }

        [Fact]
        public async Task GivenRequestWithNegativeRentalUnits_WhenPut_ThenFailWithSpecificMessage()
        {
            var request = new RentalUpdateBindingModel()
            {
                Units = -10,
                PreparationTimeInDays = 2
            };

            Func<Task> act =
                async () => await _client.PutAsJsonAsync($"/api/v1/rentals/{SomeRentalId}", request);

            await act.Should().ThrowAsync<ApplicationException>()
                .WithMessage("Units must be positive");
        }
        
        [Fact]
        public async Task GivenRequestWithNegativeRentalPreparationDays_WhenPut_ThenFailWithSpecificMessage()
        {
            var request = new RentalUpdateBindingModel()
            {
                Units = 10,
                PreparationTimeInDays = -2
            };

            Func<Task> act =
                async () => await _client.PutAsJsonAsync($"/api/v1/rentals/{SomeRentalId}", request);

            await act.Should().ThrowAsync<ApplicationException>()
                .WithMessage("Preparation Time must be positive");
        }

        [Fact]
        public async Task GivenOkRequestWithOnlyUnits_WhenPut_ThenSuccess()
        {
            var rentalResourceId = 
                await GivenIdForSuccessfulRentalPostWith(noOfUnits: 2, 10,_client);
     
            var request = new RentalUpdateBindingModel()
            {
                Units = NewRentalUnitsValue
            };
            
            using (var putResponse = await  _client.PutAsJsonAsync($"/api/v1/rentals/{rentalResourceId.Id}", request))
            {
                putResponse.IsSuccessStatusCode.Should().BeTrue();
            }

            using (var getResponse = await _client.GetAsync($"/api/v1/rentals/{rentalResourceId.Id}"))
            {
                getResponse.IsSuccessStatusCode.Should().BeTrue();

                var getResult = await getResponse.Content.ReadAsAsync<RentalViewModel>();
                getResult.Units.Should().Be(NewRentalUnitsValue);
            }
        }
        
        [Fact]
        public async Task GivenOkRequestWithNewPreparationsNotOverlapping_WhenPut_ThenSuccess()
        {
            var rentalResourceId = 
                await GivenIdForSuccessfulRentalPostWith(noOfUnits: 1, 4,_client);
            
            var postBooking1Request = new BookingBindingModel
            {
                RentalId = rentalResourceId.Id,
                Nights = 1,
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
                Start = new DateTime(2002, 01, 10)
            };
            
            using (var postBooking2Response = 
                   await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking2Request))
            {
                postBooking2Response.IsSuccessStatusCode.Should().BeTrue();
            }
            
            var request = new RentalUpdateBindingModel()
            {
                PreparationTimeInDays = 6
            };

            using (var putResponse = await  _client.PutAsJsonAsync($"/api/v1/rentals/{rentalResourceId.Id}", request))
            {
                putResponse.IsSuccessStatusCode.Should().BeTrue();
            }
        }
        
        [Fact]
        public async Task GivenOkRequestWithNewOverlappingPreparations_WhenPut_ThenErrorWithSpecificMessages()
        {
            var rentalResourceId = 
                await GivenIdForSuccessfulRentalPostWith(noOfUnits: 1, 4,_client);
            
            var postBooking1Request = new BookingBindingModel
            {
                RentalId = rentalResourceId.Id,
                Nights = 1,
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
                Start = new DateTime(2002, 01, 10)
            };
            
            using (var postBooking2Response = 
                   await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking2Request))
            {
                postBooking2Response.IsSuccessStatusCode.Should().BeTrue();
            }
            
            var request = new RentalUpdateBindingModel()
            {
                PreparationTimeInDays = 20
            };
            
            Func<Task> act =
                async () => await _client.PutAsJsonAsync($"/api/v1/rentals/{rentalResourceId.Id}", request);

            await act.Should().ThrowAsync<ApplicationException>()
                .WithMessage("Preparation overlapping");
        }
    }
}