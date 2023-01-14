using System.Threading.Tasks;
using Autofac.Extras.Moq;
using FluentAssertions;
using Moq;
using VacationRental.Application.Abstractions;
using VacationRental.Application.Services;
using VacationRental.Domain;
using Xunit;

namespace VacationRental.Application.Tests
{
    public class RentalServiceTests
    {
        private const int SomeRentalId = 1234;
        private const int SomeRentalUnitsValue = 2392838;
        
        [Fact]
        public async Task GivenDataMissing_WhenGetById_ThenShouldReturnErrorContext()
        {
            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IRentalRepository>()
                    .Setup(r => r.GetById(It.IsAny<int>()))
                    .ReturnsAsync(() => null);
                
                var sut = mock.Create<RentalService>();

                var result = await sut.GetById(It.IsAny<int>());

                result.Execution.IsSuccess.Should().BeFalse();
                result.Execution.Error.Message.Should().Be("Rental not found");
            }
        }

        [Fact]
        public async Task GivenDataExists_WhenGetById_ThenDataShouldBeAvailable()
        {
            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IRentalRepository>()
                    .Setup(r => r.GetById(It.IsAny<int>()))
                    .ReturnsAsync(
                        new Rental()
                        {
                            Id =SomeRentalId,
                            Units = SomeRentalUnitsValue
                        });

                var sut = mock.Create<RentalService>();

                var result = await sut.GetById(It.IsAny<int>());

                result.Execution.IsSuccess.Should().BeTrue();
                result.Data.Id.Should().Be(SomeRentalId);
                result.Data.Units.Should().Be(SomeRentalUnitsValue);
            }
        }
       
    }
}