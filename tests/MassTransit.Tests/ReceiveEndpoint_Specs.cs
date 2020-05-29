namespace MassTransit.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Creating_a_receive_endpoint_from_an_existing_bus :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_be_allowed()
        {
            Task<ConsumeContext<PingMessage>> pingHandled = null;

            var handle = Bus.ConnectReceiveEndpoint("second_queue", x =>
            {
                pingHandled = Handled<PingMessage>(x);
            });
            await handle.Ready;
            try
            {
                await Bus.Publish(new PingMessage());

                ConsumeContext<PingMessage> pinged = await pingHandled;

                Assert.That(pinged.ReceiveContext.InputAddress, Is.EqualTo(new Uri("loopback://localhost/second_queue")));
            }
            finally
            {
                await handle.StopAsync();
            }
        }
    }


    [TestFixture]
    public class Creating_a_receive_endpoint_from_an_existing_bus_twice :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_not_be_allowed_twice()
        {
            Task<ConsumeContext<PingMessage>> pingHandled = null;
            var handle = Bus.ConnectReceiveEndpoint("second_queue", x =>
            {
                pingHandled = Handled<PingMessage>(x);
            });
            await handle.Ready;
            try
            {
                Assert.That(async () =>
                {
                    var unused = Bus.ConnectReceiveEndpoint("second_queue", x =>
                    {
                    });
                    await unused.Ready;
                }, Throws.TypeOf<ConfigurationException>());
            }
            finally
            {
                await handle.StopAsync();
            }
        }
    }
}
