namespace MassTransit.TestFramework.ForkJoint.Tests
{
    using System;
    using System.Threading.Tasks;
    using Consumers;
    using Contracts;
    using Futures;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Services;


    [TestFixture]
    public class FryShakeFuture_Specs :
        FutureTestFixture
    {
        [Test]
        public async Task Should_complete()
        {
            var orderId = NewId.NextGuid();
            var orderLineId = NewId.NextGuid();

            var startedAt = DateTime.UtcNow;

            var scope = Provider.CreateScope();

            var client = scope.ServiceProvider.GetRequiredService<IRequestClient<OrderFryShake>>();

            Response<FryShakeCompleted> response = await client.GetResponse<FryShakeCompleted>(new
            {
                OrderId = orderId,
                OrderLineId = orderLineId,
                Flavor = "Chocolate",
                Size = Size.Medium
            });

            Assert.That(response.Message.OrderId, Is.EqualTo(orderId));
            Assert.That(response.Message.OrderLineId, Is.EqualTo(orderLineId));
            Assert.That(response.Message.Size, Is.EqualTo(Size.Medium));
            Assert.That(response.Message.Created, Is.GreaterThan(startedAt));
            Assert.That(response.Message.Completed, Is.GreaterThan(response.Message.Created));
            Assert.That(response.Message.Description, Contains.Substring("FryShake(2)"));
        }

        [Test]
        public async Task Should_fault()
        {
            var orderId = NewId.NextGuid();
            var orderLineId = NewId.NextGuid();

            var scope = Provider.CreateScope();

            var client = scope.ServiceProvider.GetRequiredService<IRequestClient<OrderFryShake>>();

            try
            {
                await client.GetResponse<FryShakeCompleted>(new
                {
                    OrderId = orderId,
                    OrderLineId = orderLineId,
                    Flavor = "Strawberry",
                    Size = Size.Large
                }, timeout: TestHarness.TestTimeout);

                Assert.Fail("Should have thrown");
            }
            catch (RequestFaultException exception)
            {
                Assert.That(exception.Fault.Host, Is.Not.Null);
                Assert.That(exception.Message, Contains.Substring("Strawberry is not available"));
            }
        }

        protected override void ConfigureServices(IServiceCollection collection)
        {
            collection.AddSingleton<IShakeMachine, ShakeMachine>();
            collection.AddSingleton<IFryer, Fryer>();
        }

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<PourShakeConsumer>();
            configurator.AddConsumer<CookFryConsumer, CookFryConsumerDefinition>();

            configurator.AddFuture<FryFuture>();
            configurator.AddFuture<ShakeFuture>();
            configurator.AddFuture<FryShakeFuture>();
        }

        public FryShakeFuture_Specs(IFutureTestFixtureConfigurator testFixtureConfigurator)
            : base(testFixtureConfigurator)
        {
        }
    }
}
