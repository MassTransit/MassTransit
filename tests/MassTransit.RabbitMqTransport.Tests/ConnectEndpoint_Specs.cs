namespace MassTransit.RabbitMqTransport.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Monitoring.Health;
    using NUnit.Framework;
    using TestFramework.Messages;


    [Explicit]
    [TestFixture]
    public class Connecting_receive_endpoints :
        RabbitMqTestFixture
    {
        BusHealth _busHealth;

        [Test]
        public async Task Should_clean_up_properly()
        {
            async Task ConnectAndRequest()
            {
                var handle = Bus.ConnectResponseEndpoint();

                await handle.Ready;

                await using var clientFactory = await handle.CreateClientFactory();

                using RequestHandle<PingMessage> requestHandle = clientFactory.CreateRequest(new PingMessage());

                await requestHandle.GetResponse<PongMessage>();
            }

            for (int i = 0; i < 10; i++)
            {
                await ConnectAndRequest();
            }

            var health = _busHealth.CheckHealth();

            foreach (KeyValuePair<string, EndpointHealthResult> healthEndpoint in health.Endpoints)
            {
                TestContext.WriteLine("Endpoint: {0}, Status: {1}", healthEndpoint.Key, healthEndpoint.Value.Description);
            }
        }

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            _busHealth = new BusHealth();

            configurator.ConnectBusObserver(_busHealth);
            configurator.ConnectEndpointConfigurationObserver(_busHealth);
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            configurator.Consumer<Consumer>();
        }


        class Consumer :
            IConsumer<PingMessage>
        {
            public async Task Consume(ConsumeContext<PingMessage> context)
            {
                await Task.Delay(1000);

                await context.RespondAsync(new PongMessage(context.Message.CorrelationId));
            }
        }
    }
}
