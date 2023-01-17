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
        [Fact]
        public async Task GivenRentalFails_WhenCreate_ThenFail()
        {
            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IRentalRepository>()
                    .Setup(r => r.Create(It.IsAny<Rental>()))
                    .ReturnsAsync(false);

                var sut = mock.Create<RentalService>();

                var result = await sut.Create(It.IsAny<Rental>());
                
                result.Should().BeFalse();
            }
        }

        [Fact]
        public async Task GivenRentalWithTwoUnitsInsertSuccess_WhenCreate_ThenInsertTwoUnits()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var input = new Rental()
                {
                    Units = 2,
                    PreparationTimeInDays = 0
                };
                
                mock.Mock<IRentalRepository>()
                    .Setup(
                        r => r.Create(It.IsAny<Rental>()))
                    .ReturnsAsync(true);
                mock.Mock<IUnitRepository>()
                    .Setup(r => r.Create(It.IsAny<Unit>()))
                    .ReturnsAsync(true);

                var sut = mock.Create<RentalService>();

                await sut.Create(input);

                var unitMock = mock.Mock<IUnitRepository>();
                
                unitMock.Verify(r=>r.Create(It.IsAny<Unit>()),Times.Exactly(2));
            }
        }
        
    }
}