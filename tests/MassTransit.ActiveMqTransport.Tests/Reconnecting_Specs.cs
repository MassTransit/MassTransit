namespace MassTransit.ActiveMqTransport.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework.Messages;
    using Testing;
    using Transports;


    [TestFixture(ActiveMqHostAddress.ActiveMqScheme)]
    [TestFixture(ActiveMqHostAddress.AmqpScheme)]
    public class Reconnecting_Specs :
        ActiveMqTestFixture
    {
        public Reconnecting_Specs(string protocol)
            : base(protocol)
        {
        }

        [Test]
        [Explicit]
        public async Task Should_fault_nicely()
        {
            await Bus.Publish(new ReconnectMessage { Value = "Before" });

            var beforeFound = await Task.Run(() => _consumer.Received.Select<ReconnectMessage>(x => x.Context.Message.Value == "Before").Any());
            Assert.That(beforeFound, Is.True);

            Console.WriteLine("Okay, restart ActiveMQ");

            for (var i = 0; i < 20; i++)
            {
                await Task.Delay(1000);

                Console.Write($"{i}. ");

                var clientFactory = Bus.CreateClientFactory(TestTimeout);

                RequestHandle<PingMessage> request = clientFactory.CreateRequest(new PingMessage());

                Response<PongMessage> response = await request.GetResponse<PongMessage>();

                if (clientFactory is IAsyncDisposable asyncDisposable)
                    await asyncDisposable.DisposeAsync();
            }

            Console.WriteLine("");
            Console.WriteLine("Resuming");

            await Bus.Publish(new ReconnectMessage { Value = "After" });

            var afterFound = await Task.Run(() => _consumer.Received.Select<ReconnectMessage>(x => x.Context.Message.Value == "After").Any());
            Assert.That(afterFound, Is.True);
        }

        public Reconnecting_Specs()
        {
            SendEndpointCacheDefaults.MinAge = TimeSpan.FromSeconds(2);
            SendEndpointCacheDefaults.Capacity = 5;
        }

        ReconnectConsumer _consumer;

        protected override void ConfigureActiveMqReceiveEndpoint(IActiveMqReceiveEndpointConfigurator configurator)
        {
            base.ConfigureActiveMqReceiveEndpoint(configurator);

            _consumer = new ReconnectConsumer(TestTimeout);

            _consumer.Configure(configurator);

            configurator.Handler<PingMessage>(context => context.RespondAsync(new PongMessage(context.Message.CorrelationId)));
        }


        class ReconnectConsumer :
            MultiTestConsumer
        {
            public ReconnectConsumer(TimeSpan timeout)
                : base(timeout)
            {
                Consume<ReconnectMessage>();
            }
        }


        public class ReconnectMessage
        {
            public string Value { get; set; }
        }
    }
}
