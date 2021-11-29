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
    public class ShakeFuture_Specs :
        FutureTestFixture
    {
        [Test]
        public async Task Should_complete()
        {
            var orderId = NewId.NextGuid();
            var orderLineId = NewId.NextGuid();

            var startedAt = DateTime.UtcNow;

            var scope = Provider.CreateScope();

            var client = scope.ServiceProvider.GetRequiredService<IRequestClient<OrderShake>>();

            Response<ShakeCompleted> response = await client.GetResponse<ShakeCompleted>(new
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
        }

        [Test]
        public async Task Should_fault()
        {
            var orderId = NewId.NextGuid();
            var orderLineId = NewId.NextGuid();

            var scope = Provider.CreateScope();

            var client = scope.ServiceProvider.GetRequiredService<IRequestClient<OrderShake>>();

            try
            {
                await client.GetResponse<ShakeCompleted>(new
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
        }

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<PourShakeConsumer>();

            configurator.AddFuture<ShakeFuture>();
        }

        public ShakeFuture_Specs(IFutureTestFixtureConfigurator testFixtureConfigurator)
            : base(testFixtureConfigurator)
        {
        }
    }
}
