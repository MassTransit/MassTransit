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


        public interface SubmitActivity
        {
            string Value { get; }
        }


        public interface ActivityCompleted
        {
            string Value { get; }
            string Variable { get; }
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
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddConsumer<TestingHarnessSubmitOrderConsumer>();

                    x.UsingRabbitMq((context, cfg) => cfg.ConfigureEndpoints(context));
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

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


        class TestingHarnessSubmitOrderConsumer :
            IConsumer<SubmitOrder>
        {
            public Task Consume(ConsumeContext<SubmitOrder> context)
            {
                return context.RespondAsync<OrderSubmitted>(context.Message);
            }
        }
    }


    [TestFixture]
    public class When_using_a_connection_factory_refresh_callback
    {
        [Test]
        public async Task Should_refresh_credentials_prior_to_connecting()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddConsumer<TestingHarnessSubmitOrderConsumer>();

                    x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host("localhost", h =>
                        {
                            h.Username("totally-bogus");
                            h.Password("not-real-at-all");

                            h.OnRefreshConnectionFactory = async factory =>
                            {
                                factory.UserName = "guest";
                                factory.Password = "guest";
                            };
                        });

                        cfg.ConfigureEndpoints(context);
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

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


        class TestingHarnessSubmitOrderConsumer :
            IConsumer<SubmitOrder>
        {
            public Task Consume(ConsumeContext<SubmitOrder> context)
            {
                return context.RespondAsync<OrderSubmitted>(context.Message);
            }
        }
    }
}
