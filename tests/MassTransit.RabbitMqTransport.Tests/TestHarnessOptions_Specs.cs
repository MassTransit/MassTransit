namespace MassTransit.RabbitMqTransport.Tests
{
    using System.Threading.Tasks;
    using HarnessContracts;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Testing;


    [TestFixture]
    public class Configuring_the_test_harness_options
    {
        [Test]
        public async Task Should_clean_the_virtual_host()
        {
            await using var provider = new ServiceCollection()
                .ConfigureRabbitMqTestOptions(r =>
                {
                    r.CreateVirtualHostIfNotExists = true;
                    r.CleanVirtualHost = true;
                })
                .AddMassTransitTestHarness(x =>
                {
                    x.AddConsumer<TestingHarnessSubmitOrderConsumer>();

                    x.UsingRabbitMq((context, cfg) => cfg.ConfigureEndpoints(context));

                    x.AddOptions<RabbitMqTransportOptions>()
                        .Configure(options => options.VHost = "test2");
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

            await Assert.MultipleAsync(async () =>
            {
                Assert.That(await harness.Sent.Any<OrderSubmitted>(), Is.True);

                Assert.That(await harness.Consumed.Any<SubmitOrder>(), Is.True);
            });

            IReceivedMessage<SubmitOrder> message = await harness.Consumed.SelectAsync<SubmitOrder>().First();

            Assert.That(message.Context.ReceiveContext.InputAddress.AbsolutePath, Contains.Substring("test2"));
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
