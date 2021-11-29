namespace MassTransit.RabbitMqTransport.Tests
{
    using System.Threading.Tasks;
    using HarnessContracts;
    using MassTransit.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;


    namespace HarnessContracts
    {
        using System;


        public interface SubmitOrder
        {
            Guid OrderId { get; }
            string OrderNumber { get; }
        }


        public interface OrderSubmitted
        {
            Guid OrderId { get; }
            string OrderNumber { get; }
        }
    }


    [TestFixture]
    public class Using_the_generic_request_client_from_the_harness
    {
        [Test]
        public async Task Should_properly_resolve_within_the_scope()
        {
            var collection = new ServiceCollection();

            await using var provider = collection
                .AddMassTransitTestHarness(x =>
                {
                    x.AddConsumer<SubmitOrderConsumer>();

                    x.UsingRabbitMq((context, cfg) => cfg.ConfigureEndpoints(context));
                })
                .BuildServiceProvider(true);

            var harness = provider.GetRequiredService<ITestHarness>();

            await harness.Start();

            IRequestClient<SubmitOrder> client = harness.GetRequestClient<SubmitOrder>();

            await client.GetResponse<OrderSubmitted>(new
            {
                OrderId = InVar.Id,
                OrderNumber = "123"
            });

            Assert.IsTrue(await harness.Sent.Any<OrderSubmitted>());

            Assert.IsTrue(await harness.Consumed.Any<SubmitOrder>());
        }


        class SubmitOrderConsumer :
            IConsumer<SubmitOrder>
        {
            public Task Consume(ConsumeContext<SubmitOrder> context)
            {
                return context.RespondAsync<OrderSubmitted>(context.Message);
            }
        }
    }
}
