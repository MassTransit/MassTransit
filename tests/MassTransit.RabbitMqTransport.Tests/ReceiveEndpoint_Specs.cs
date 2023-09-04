namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class Creating_a_receive_endpoint_from_an_existing_bus :
        RabbitMqTestFixture
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

                Assert.That(pinged.ReceiveContext.InputAddress, Is.EqualTo(new Uri(HostAddress, "second_queue")));
            }
            finally
            {
                await handle.StopAsync();
            }
        }

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

    [TestFixture]
    public class Disconnecting_a_receive_endpoint_from_an_existing_bus :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_be_allowed()
        {
            Task<ConsumeContext<PingMessage>> pingHandled = null;

            var handle = Bus.ConnectReceiveEndpoint("second_queue");
            await handle.Ready;

            try
            {
                var disconnected = await Bus.DisconnectReceiveEndpoint("second_queue");

                Assert.AreEqual(disconnected, true);
            }
            finally
            {
                await handle.StopAsync();
            }
        }

        [Test]
        public async Task Should_not_be_allowed_when_it_is_not_exist()
        {
            Assert.ThrowsAsync<ConfigurationException>(() => Bus.DisconnectReceiveEndpoint("second_queue"));
        }
    }
}
