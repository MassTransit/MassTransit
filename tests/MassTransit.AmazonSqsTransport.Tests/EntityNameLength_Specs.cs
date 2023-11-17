namespace MassTransit.AmazonSqsTransport.Tests
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework.Messages;
    using Testing;


    [TestFixture]
    public class Entity_name_validation
    {
        [Test]
        public async Task Should_throw_on_queue_name_length_exceeded()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddConsumer<DumbConsumer>()
                        .Endpoint(e => e.Name = new string('a', 81));

                    x.UsingAmazonSqs((context, cfg) =>
                    {
                        cfg.LocalstackHost();

                        cfg.ConfigureEndpoints(context);
                    });
                })
                .BuildServiceProvider(true);

            Assert.That(async () => await provider.StartTestHarness(), Throws.TypeOf<ConfigurationException>());
        }


        class DumbConsumer :
            IConsumer<PingMessage>
        {
            public Task Consume(ConsumeContext<PingMessage> context)
            {
                return Task.CompletedTask;
            }
        }
    }
}
