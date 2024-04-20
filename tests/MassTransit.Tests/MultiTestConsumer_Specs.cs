namespace MassTransit.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Using_the_multi_test_consumer :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_distinguish_multiple_events()
        {
            var consumer = new PingPongConsumer(TestTimeout);

            var handle = Bus.ConnectReceiveEndpoint("boring", x => consumer.Configure(x));
            await handle.Ready;
            try
            {
                var pingMessage = new PingMessage();
                var pingMessage2 = new PingMessage();
                await Bus.Publish(pingMessage);
                await Bus.Publish(pingMessage2);

                Assert.Multiple(() =>
                {
                    Assert.That(consumer.Received.Select<PingMessage>(received => received.Context.Message.CorrelationId == pingMessage.CorrelationId).Any(),
                        Is.True);
                    Assert.That(consumer.Received.Select<PingMessage>(received => received.Context.Message.CorrelationId == pingMessage2.CorrelationId).Any(),
                        Is.True);
                });
            }
            finally
            {
                await handle.StopAsync();
            }
        }

        [Test]
        public async Task Should_show_that_the_message_was_received_by_the_consumer()
        {
            var multiConsumer = new MultiTestConsumer(TestTimeout);
            ReceivedMessageList<PingMessage> received = multiConsumer.Consume<PingMessage>();

            var handle = Bus.ConnectReceiveEndpoint("boring2", x => multiConsumer.Configure(x));
            await handle.Ready;

            try
            {
                await Bus.Publish(new PingMessage());

                Assert.That(received.Select().Any(), Is.True);
            }
            finally
            {
                await handle.StopAsync();
            }
        }

        [Test]
        public async Task Should_show_that_the_specified_type_was_received()
        {
            var consumer = new PingPongConsumer(TestTimeout);

            var handle = Bus.ConnectReceiveEndpoint("boring3", x => consumer.Configure(x));
            await handle.Ready;

            try
            {
                var pingMessage = new PingMessage();
                await Bus.Publish(pingMessage);
                await Bus.Publish(new PongMessage(pingMessage.CorrelationId));

                Assert.Multiple(() =>
                {
                    Assert.That(consumer.Received.Select<PingMessage>().Any(), Is.True);
                    Assert.That(consumer.Received.Select<PongMessage>(received => received.Context.Message.CorrelationId == pingMessage.CorrelationId).Any(),
                        Is.True);
                });
            }
            finally
            {
                await handle.StopAsync();
            }
        }


        class PingPongConsumer :
            MultiTestConsumer
        {
            public PingPongConsumer(TimeSpan timeout)
                : base(timeout)
            {
                Consume<PingMessage>();
                Consume<PongMessage>();
            }
        }
    }
}
