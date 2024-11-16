using Bogus;

namespace SampleDataGeneration.Api.Features.Orders;

public class OrderFaker
{
    private Faker<Order>? _orderFaker;

    private Faker<BillingDetails>? _billingDetailsFaker;

    public const int RandomSeed = 123456;

    public Faker<Order> GetOrderGenerator()
    {
        Faker<BillingDetails> addressFaker = GetAddressGenerator();

        _orderFaker ??= new Faker<Order>()
            .UseSeed(RandomSeed)
            .RuleFor(o => o.Id, Guid.NewGuid())
            .RuleFor(o => o.Currency, f => f.Finance.Currency().Code)
            .RuleFor(o => o.Price, f => f.Finance.Amount())
            .RuleFor(o => o.BillingDetails, f => addressFaker);

        return _orderFaker!;

        //foreach (var item in orderFaker.GenerateForever())
        //{
        //    var orderText = JsonSerializer.Serialize(orderFaker.Generate());
        //    Console.WriteLine(orderText);
        //    await Task.Delay(1000);
        //}
    }

    private Faker<BillingDetails> GetAddressGenerator()
    {
        _billingDetailsFaker ??= new Faker<BillingDetails>()
            .UseSeed(RandomSeed)
            .RuleFor(b => b.CustomerName, f => f.Person.FullName)
            .RuleFor(b => b.Email, f => f.Person.Email)
            .RuleFor(b => b.AddressLine, f => f.Address.StreetAddress())
            .RuleFor(b => b.City, f => f.Address.City())
            .RuleFor(b => b.Country, f => f.Address.Country())
            .RuleFor(b => b.PostalCode, f => f.Address.ZipCode())
            .RuleFor(b => b.Phone, f => f.Phone.PhoneNumberFormat().OrNull(f, .15f));

        return _billingDetailsFaker;
    }
}