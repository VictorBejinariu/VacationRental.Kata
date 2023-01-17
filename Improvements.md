## Approach:

I wanted to test everything through unit tests and integration tests.

Aim to achieve over 80% overall code coverage. (Currently 91% Coverage)

Find the core business and the modules most likely to change over time. Write 100% SOLID. Create abstractions. Isolate and achieve over 90% code coverage on it. (Currently 92% on ApplicationLayer)

Try to spot functionalities that can be used across company and write code as nugets.

Refactor a lot but only modules of code with over 90% code coverage in order to ensure existing functionality is not broken.

Write fluent Code when business is complex.

Well defined boundaries between layers. Each Layer exposes only Contracts and Abstractions namespaces. Contracts is used by clients(IRentalHandler). Abstractions by Layers of implementations(e.g. IRentalRepository). Concrete Implementations are internal.

Don't use magic stuff MediatR, FluentValidation, Attributes unless really applicable. It's true it would've been nice to separate Command from Query at least for the Booking Flow.

Define Business Entities

Mix DomainModel approach(e.g. BookingHandler) witih TransactionScript (Spagetti Code) approach (e.g. CallendarController).

#### KeepItSimpleStupid, FEATURE FIRST ! ! !

## Cool examples:

### Fluent Code


```c#
return RequestHandler<Rental>
    .New()
    .With(Error.WithMessage(RentalNotFoundErrorMessage));
```

```c#
return RequestHandler<Rental>.New().With(rental);
```

This is something actually cool: I've spent 6 to 10 hours on doing this.
I've build "class steps", I've build sync steps. This was the first time I've achieved fluent asnyc method sptes.

```c#
BookingCreateAsyncContextBuilder.From(input)
                .AndThenAlways(Validate)
                .AndThenTry(ReadCorrespondingRental)
                .AndThenTry(CreateBooking)
                .AndThenTry(SelectAvailableUnit)
                .AndThenTry(PersistBooking)
                .Run()
```

```c#
getCalendarResult.Dates[3].PreparationTimes
    .Should()
    .Contain(x => x.UnitId == 1)
    .And.Contain(x => x.UnitId == 2);
```
```c#
if (!retrieval.Execution.IsSuccess)
{
    throw new ApplicationException(retrieval.Execution.Error.Message);
}
```

### Open for extension

```c#
 public BookingHandler(
            //...
            IEnumerable<IBookingActualNightsHydrator> actualNightsHydrators,
            IEnumerable<IBookingActualStartHydrator> actualStartHydrators){
            //...
            }
```

```c#
public interface IBookingActualNightsHydrator
{
    string Key { get;}
    Task<int> Hydrate(Booking booking, int previousValue);
}
```

### Quick Unit Tests with Automock Moq and FluentAssertions

```c#
[Fact]
public async Task GivenDataMissing_WhenGetById_ThenShouldReturnErrorContext()
{
    using (var mock = AutoMock.GetLoose())
    {
        mock.Mock<IRentalRepository>()
            .Setup(r => r.GetById(It.Is<int>(i=>i==IdForSomeMissingRental)))
            .ReturnsAsync(() => null);
        
        var sut = mock.Create<RentalHandler>();

        var result = await sut.GetById(IdForSomeMissingRental);

        result.Execution.IsSuccess.Should().BeFalse();
        result.Execution.Error.Message.Should().Be("Rental not found");
    }
}
```
### many POCOs, many Mappers
```c#
public class Preparation
{
    public int Id { get; set; }
    public int RentalId { get; set; }
    public int UnitId { get; set; }
    public int BookingId { get; set; }
    public DateTime Start { get; set; }
    public int Nights { get; set; }
}
```


## TODO:
95% Code Coverage

Reactive, Raise Events (e.g. Booking Created)

Rethink the Domain.Entities. Achieve a more DDD.

...





