using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using VacationRental.Api.Models;
using Xunit;

namespace VacationRental.Api.Tests
{
    [Collection("Integration")]
    public class PostRentalTests
    {
        private readonly HttpClient _client;

        public PostRentalTests(IntegrationFixture fixture)
        {
            _client = fixture.Client??throw new ArgumentNullException(nameof(fixture.Client));
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenPostRental_ThenAGetReturnsTheCreatedRental()
        {
            var request = new RentalBindingModel
            {
                Units = 25,
                PreparationTimeInDays = 2
            };

            ResourceIdViewModel postResult;
            using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", request))
            {
                postResponse.IsSuccessStatusCode.Should().BeTrue();
                postResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            using (var getResponse = await _client.GetAsync($"/api/v1/rentals/{postResult.Id}"))
            {
                getResponse.IsSuccessStatusCode.Should().BeTrue();

                var getResult = await getResponse.Content.ReadAsAsync<RentalViewModel>();
                getResult.Units.Should().Be(request.Units);
                getResult.PreparationTimeInDays.Should().Be(request.PreparationTimeInDays);
            }
        }
        
        [Fact]
        public async Task GivenRequestWithoutPreparation_WhenPostRental_ThenAGetReturns0Preparation()
        {
            var request = new RentalBindingModel
            {
                Units = 25
            };

            ResourceIdViewModel postResult;
            using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", request))
            {
                postResponse.IsSuccessStatusCode.Should().BeTrue();
                postResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            using (var getResponse = await _client.GetAsync($"/api/v1/rentals/{postResult.Id}"))
            {
                getResponse.IsSuccessStatusCode.Should().BeTrue();

                var getResult = await getResponse.Content.ReadAsAsync<RentalViewModel>();
                getResult.PreparationTimeInDays.Should().Be(0);
            }
        }
    }
}
