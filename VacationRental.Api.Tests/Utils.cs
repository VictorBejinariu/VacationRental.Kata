using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using VacationRental.Api.Models;

namespace VacationRental.Api.Tests
{
    public static class Utils
    {
        public static async Task<ResourceIdViewModel> GivenIdForSuccessfulRentalPostWith(int noOfUnits, HttpClient client)
        {
            var postRentalRequest = new RentalBindingModel
            {
                Units = noOfUnits
            };

            ResourceIdViewModel postRentalResult;
            using (var postRentalResponse = 
                   await client.PostAsJsonAsync($"/api/v1/rentals", postRentalRequest))
            {
                postRentalResponse.IsSuccessStatusCode.Should().Be(true);
                postRentalResult = await postRentalResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            return postRentalResult;
        }
    }
}