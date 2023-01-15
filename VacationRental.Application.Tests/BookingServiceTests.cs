using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using FluentAssertions;
using Moq;
using VacationRental.Application.Abstractions;
using VacationRental.Application.Contracts;
using VacationRental.Application.Services;
using VacationRental.Domain;
using Xunit;

namespace VacationRental.Application.Tests
{
    public class BookingServiceTests
    {
        private const int IdForMissingBooking = 293349;
        private const int IdForMissingRental = 887;
        private const int IdForExistingBooking = 2394;
        private const int SomeNegativeValueForNights = -2308;
        private const int SomeRentalId = 1;
        private const int SomeNightsValue = 3;
        private static readonly DateTime SomeStartDate = new DateTime(103499384);

        [Fact]
        public async Task GivenDataMissing_WhenGetById_ThenShouldReturnError()
        {
            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IBookingRepository>()
                    .Setup(r => r.GetById(It.Is<int>(i=>i==IdForMissingBooking)))
                    .ReturnsAsync((Booking) null);

                var sut = mock.Create<BookingService>();

                var result = await sut.GetById(IdForMissingBooking);

                result.Execution.IsSuccess.Should().BeFalse();
                result.Execution.Error.Message.Should().Be("Booking not found");
            }
        }

        [Fact]
        public async Task GivenDataExists_WhenGetById_ReturnsData()
        {
            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IBookingRepository>()
                    .Setup(r => r.GetById(It.Is<int>(i => i == IdForExistingBooking)))
                    .ReturnsAsync(new Booking()
                    {
                        Id = IdForExistingBooking,
                        Nights = 3,
                        Start = new DateTime(100000000),
                        RentalId = 1
                    });

                var sut = mock.Create<BookingService>();

                var result = await sut.GetById(IdForExistingBooking);

                result.Execution.IsSuccess.Should().BeTrue();
                result.Data.Id.Should().Be(IdForExistingBooking);
                result.Data.RentalId.Should().Be(1);
                result.Data.Start.Should().Be(new DateTime(100000000));
                result.Data.Nights.Should().Be(3);
            }
        }

        [Fact]
        public async Task GivenNegativeNights_WhenCreateBooking_ThenShouldReturnErrorWithSpecificMessage()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var input = GivenBookingWith(SomeRentalId, SomeNegativeValueForNights, SomeStartDate);

                var sut = mock.Create<BookingService>();

                var result =await sut.Create(input);

                result.Execution.IsSuccess.Should().BeFalse();
                result.Execution.Error.Message.Should().Be("Nights must be positive");
            }
        }

        [Fact]
        public async Task GivenRequestForMissingRental_WhenCreateBooking_ThenShouldReturnErrorWithSpecificMessage()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var input = GivenBookingWith(IdForMissingRental, SomeNightsValue, SomeStartDate);
                mock.Mock<IRentalRepository>()
                    .Setup(r => r.GetById(It.Is<int>(i => i == IdForMissingRental)))
                    .ReturnsAsync((Rental) null);

                var sut = mock.Create<BookingService>();

                var result = await sut.Create(input);

                result.Execution.IsSuccess.Should().BeFalse();
                result.Execution.Error.Message.Should().Be("Rental not found");
            }
        }

        [Fact]
        public async Task GivenRentalWithTwoUnitsAndOneBooking_WhenCreateBooking_ThenShouldReturnSuccess()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var input = GivenBookingWith(SomeRentalId, 3, SomeStartDate);
                mock.Mock<IRentalRepository>()
                    .Setup(r => r.GetById(It.Is<int>(i => i == SomeRentalId)))
                    .ReturnsAsync(new Rental()
                    {
                        Id = SomeRentalId,
                        Units = 2
                    });
                mock.Mock<IBookingRepository>()
                    .Setup(r => r.Get())
                    .ReturnsAsync(new List<Booking>()
                    {
                        new Booking()
                        {
                            Id = 1,
                            RentalId = SomeRentalId,
                            Start = SomeStartDate.AddDays(1),
                            Nights = 3
                        }
                    });
                mock.Mock<IBookingRepository>()
                    .Setup(r => r.Create(It.IsAny<Booking>()))
                    .ReturnsAsync(true);

                var sut = mock.Create<BookingService>();

                var result = await sut.Create(input);

                result.Execution.IsSuccess.Should().BeTrue();
            }
        }

        [Theory]
        [MemberData(nameof(OverlappingBookings))]
        public async Task GivenBookingOverlapping_WhenCreateBooking_ThenShouldReturnErrorWithSpecificError(
            Booking existingBooking, BookingCreate newBooking)
        {
            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IRentalRepository>()
                    .Setup(r => r.GetById(It.Is<int>(i => i == SomeRentalId)))
                    .ReturnsAsync(new Rental()
                    {
                        Id = SomeRentalId,
                        Units = 1
                    });
                mock.Mock<IBookingRepository>()
                    .Setup(r => r.Get())
                    .ReturnsAsync(new List<Booking>()
                    {
                        existingBooking
                    });
                mock.Mock<IBookingRepository>()
                    .Setup(r => r.Create(It.IsAny<Booking>()))
                    .ReturnsAsync(true);

                var sut = mock.Create<BookingService>();

                var result = await sut.Create(newBooking);

                result.Execution.IsSuccess.Should().BeFalse();
                result.Execution.Error.Message.Should().Be("Not available");
            }
        }
        
        [Theory]
        [MemberData(nameof(OverlappingBookings))]
        public async Task GivenBookingOverlappingAndAvailableUnits_WhenCreateBooking_ThenShouldReturnSuccess(
            Booking existingBooking, BookingCreate newBooking)
        {
            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IRentalRepository>()
                    .Setup(r => r.GetById(It.Is<int>(i => i == SomeRentalId)))
                    .ReturnsAsync(new Rental()
                    {
                        Id = SomeRentalId,
                        Units = 2
                    });
                mock.Mock<IBookingRepository>()
                    .Setup(r => r.Get())
                    .ReturnsAsync(new List<Booking>()
                    {
                        existingBooking
                    });
                mock.Mock<IBookingRepository>()
                    .Setup(r => r.Create(It.IsAny<Booking>()))
                    .ReturnsAsync(true);

                var sut = mock.Create<BookingService>();

                var result = await sut.Create(newBooking);

                result.Execution.IsSuccess.Should().BeTrue();
            }
        }
        
        private BookingCreate GivenBookingWith(int rentalId, int nights, DateTime start) =>
            new BookingCreate()
            {
                RentalId = rentalId,
                Nights = nights,
                Start = start
            };
        
        public static readonly object[][] OverlappingBookings =
        {
            new object[]
            {
                new Booking()
                {
                    Id = 1,
                    RentalId = SomeRentalId,
                    Start = new DateTime(2000,1,15),
                    Nights = 5
                },
                new BookingCreate()
                {
                    RentalId = SomeRentalId,
                    Start = new DateTime(2000,1, 13),
                    Nights = 5
                }
            },
            new object[]
            {
                new Booking()
                {
                    Id = 2,
                    RentalId = SomeRentalId,
                    Start = new DateTime(2000,1,15),
                    Nights = 5
                },
                new BookingCreate()
                {
                    RentalId = SomeRentalId,
                    Start = new DateTime(2000,1, 17),
                    Nights = 5
                }
            },
            new object[]
            {
                new Booking()
                {
                    Id = 3,
                    RentalId = SomeRentalId,
                    Start = new DateTime(2000,1,15),
                    Nights = 5
                },
                new BookingCreate()
                {
                    RentalId = SomeRentalId,
                    Start = new DateTime(2000,1, 16),
                    Nights = 3
                }
            },
            new object[]
            {
                new Booking()
                {
                    Id = 3,
                    RentalId = SomeRentalId,
                    Start = new DateTime(2000,1,15),
                    Nights = 5
                },
                new BookingCreate()
                {
                    RentalId = SomeRentalId,
                    Start = new DateTime(2000,1, 12),
                    Nights = 8
                }
            }
        };
    }
}