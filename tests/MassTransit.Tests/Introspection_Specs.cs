namespace MassTransit.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Probing_the_bus :
        InMemoryTestFixture
    {
        [Test]
        public void Should_extract_receive_endpoint_addresses()
        {
            List<Uri> receiveAddresses = Bus.GetReceiveEndpointAddresses().ToList();
            Assert.That(receiveAddresses, Does.Contain(InputQueueAddress));
        }

        [Test]
        [Explicit]
        public async Task Should_return_a_wonderful_breakdown_of_the_guts_inside_it()
        {
            await Bus.Publish(new PingMessage());

            await _handled;
        }

        #pragma warning disable NUnit1032
        Task<ConsumeContext<PingMessage>> _handled;
        #pragma warning restore NUnit1032

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Handler<PingMessage>(async context =>
            {
            }, x =>
            {
                x.UseRateLimit(100, TimeSpan.FromSeconds(1));
                x.UseConcurrencyLimit(32);
            });

            _handled = Handled<PingMessage>(configurator);

            var consumer = new MultiTestConsumer(TestTimeout);

            consumer.Consume<PingMessage>();
            consumer.Consume<PongMessage>();

            consumer.Configure(configurator);
        }
    }
}
