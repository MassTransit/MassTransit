namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework.Messages;
    using Transports;


    [TestFixture]
    public class Reconnecting_Specs :
        RabbitMqTestFixture
    {
        [Test]
        [Explicit]
        public async Task Should_fault_nicely()
        {
            await Bus.Publish(new ReconnectMessage { Value = "Before" });

            var beforeFound = await Task.Run(() => _consumer.Received.Select<ReconnectMessage>(x => x.Context.Message.Value == "Before").Any());
            Assert.IsTrue(beforeFound);

            Console.WriteLine("Okay, restart RabbitMQ");

            for (var i = 0; i < 20; i++)
            {
                await Task.Delay(1000);

                Console.Write($"{i}. ");

                var clientFactory = Bus.CreateClientFactory(TestTimeout);
                try
                {
                    RequestHandle<PingMessage> request = clientFactory.CreateRequest(new PingMessage());


                    Response<PongMessage> response = await request.GetResponse<PongMessage>();
                }
                finally
                {
                    if (clientFactory is IAsyncDisposable asyncDisposable)
                        await asyncDisposable.DisposeAsync();
                }
            }

            Console.WriteLine("");
            Console.WriteLine("Resuming");

            await Bus.Publish(new ReconnectMessage { Value = "After" });

            var afterFound = await Task.Run(() => _consumer.Received.Select<ReconnectMessage>(x => x.Context.Message.Value == "After").Any());
            Assert.IsTrue(afterFound);
        }

        public Reconnecting_Specs()
        {
            SendEndpointCacheDefaults.MinAge = TimeSpan.FromSeconds(2);
            SendEndpointCacheDefaults.Capacity = 5;
        }

        ReconnectConsumer _consumer;

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            base.ConfigureRabbitMqReceiveEndpoint(configurator);

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
