using System;
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
    public class BookingServiceTests
    {
        private const int SomeRentalId = 1923;
        private const int SomePreparationDaysValue = 2;
        private const int ANight = 1;
        private const int SomeUnitId = 10000;

        [Fact]
        public async Task GivenFailureOnRepositoryCreate_WhenCreate_ThenReturnsFailed()
        {
            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IBookingRepository>()
                    .Setup(r => r.Create(It.IsAny<Booking>()))
                    .ReturnsAsync(false);

                var sut = mock.Create<BookingService>();
                
                var result = await sut.Create(It.IsAny<Booking>());

                result.Should().BeFalse();
            }
        }

        [Fact]
        public async Task GivenSuccessRepoInsert_WhenCreate_ThenPreparationShouldBeCreated()
        {
            var input = new Booking()
            {
                Id = 1,
                RentalId = SomeRentalId,
                UnitId = SomeUnitId,
                Start = new DateTime(2023,01,01),
                Nights = ANight
            };
            
            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IBookingRepository>()
                    .Setup(r => r.Create(It.IsAny<Booking>()))
                    .ReturnsAsync(true);

                mock.Mock<IRentalService>()
                    .Setup(r => r.GetById(It.IsAny<int>()))
                    .ReturnsAsync(new Rental()
                    {
                        Id = SomeRentalId,
                        Units = 1,
                        PreparationTimeInDays = SomePreparationDaysValue
                    });

                mock.Mock<IPreparationRepository>();

                var sut = mock.Create<BookingService>();

                await sut.Create(input);
                
                mock.Mock<IPreparationRepository>()
                    .Verify(r=>r.Create(
                        It.Is<Preparation>(
                            p=>p.RentalId == SomeRentalId 
                            && p.BookingId == input.Id
                            && p.UnitId == SomeUnitId
                            && p.Start == new DateTime(2023,01,03)
                            && p.Nights == SomePreparationDaysValue
                            )),Times.Once);

            }
        }
    }
}