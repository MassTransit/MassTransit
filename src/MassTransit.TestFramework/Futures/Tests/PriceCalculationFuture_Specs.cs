namespace MassTransit.TestFramework.Futures.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
    using ForkJoint.Services;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;


    [TestFixture]
    public class PriceCalculationFuture_Specs :
        FutureTestFixture
    {
        [Test]
        public async Task Should_complete()
        {
            var orderLineId = NewId.NextGuid();

            var scope = Provider.CreateScope();

            var client = scope.ServiceProvider.GetRequiredService<IRequestClient<CalculatePrice>>();

            Response<PriceCalculation> response = await client.GetResponse<PriceCalculation>(new
            {
                OrderLineId = orderLineId,
                Sku = "90210"
            }, timeout: RequestTimeout.After(s: 5));

            Assert.That(response.Message.OrderLineId, Is.EqualTo(orderLineId));
        }

        protected override void ConfigureServices(IServiceCollection collection)
        {
            collection.AddSingleton<IFryer, Fryer>();
        }

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<CalculatePriceConsumer, CalculatePriceConsumerDefinition>();

            configurator.AddFuture<PriceCalculationFuture, PriceCalculationFutureDefinition>();
        }

        public PriceCalculationFuture_Specs(IFutureTestFixtureConfigurator testFixtureConfigurator)
            : base(testFixtureConfigurator)
        {
        }
    }


    [TestFixture]
    public class PriceCalculationFuture_RegistrationSpecs :
        FutureTestFixture
    {
        [Test]
        public async Task Should_complete()
        {
            var orderLineId = NewId.NextGuid();

            var scope = ServiceProviderServiceExtensions.CreateScope(Provider);

            var client = scope.ServiceProvider.GetRequiredService<IRequestClient<CalculatePrice>>();

            Response<PriceCalculation> response = await client.GetResponse<PriceCalculation>(new
            {
                OrderLineId = orderLineId,
                Sku = "90210"
            }, timeout: RequestTimeout.After(s: 5));

            Assert.That(response.Message.OrderLineId, Is.EqualTo(orderLineId));
        }

        protected override void ConfigureServices(IServiceCollection collection)
        {
            collection.AddSingleton<IFryer, Fryer>();
        }

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddFutureRequestConsumer<PriceCalculationFuture, CalculatePriceConsumer, CalculatePrice, PriceCalculation>();
        }

        public PriceCalculationFuture_RegistrationSpecs(IFutureTestFixtureConfigurator testFixtureConfigurator)
            : base(testFixtureConfigurator)
        {
        }
    }


    [TestFixture]
    public class PriceCalculationFuture_Faulted :
        FutureTestFixture
    {
        [Test]
        public async Task Should_faulted()
        {
            var orderLineId = NewId.NextGuid();

            var scope = ServiceProviderServiceExtensions.CreateScope(Provider);

            var client = scope.ServiceProvider.GetRequiredService<IRequestClient<CalculatePrice>>();

            var exception = Assert.ThrowsAsync<RequestFaultException>(async () => await client.GetResponse<PriceCalculation>(new
            {
                OrderLineId = orderLineId,
                Sku = "missing"
            }, timeout: RequestTimeout.After(s: 5)));

            Assert.That(exception.Fault.Exceptions.Any(x => x.ExceptionType == TypeCache<IntentionalTestException>.ShortName));
        }

        protected override void ConfigureServices(IServiceCollection collection)
        {
            collection.AddSingleton<IFryer, Fryer>();
        }

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddFutureRequestConsumer<PriceCalculationFuture, CalculatePriceConsumer, CalculatePrice, PriceCalculation>();
        }

        public PriceCalculationFuture_Faulted(IFutureTestFixtureConfigurator testFixtureConfigurator)
            : base(testFixtureConfigurator)
        {
        }
    }
}
