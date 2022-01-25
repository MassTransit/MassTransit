namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework.Messages;


    [Explicit]
    [TestFixture]
    public class Connecting_receive_endpoints :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_clean_up_properly()
        {
            async Task ConnectAndRequest()
            {
                var handle = Bus.ConnectResponseEndpoint();

                await handle.Ready;

                var clientFactory = await handle.CreateClientFactory();
                try
                {
                    using RequestHandle<PingMessage> requestHandle = clientFactory.CreateRequest(new PingMessage());

                    await requestHandle.GetResponse<PongMessage>();

                    await handle.StopAsync(TestCancellationToken);
                }
                finally
                {
                    if (clientFactory is IAsyncDisposable asyncDisposable)
                        await asyncDisposable.DisposeAsync();
                }
            }

            for (var i = 0; i < 10; i++)
                await ConnectAndRequest();

            var health = BusControl.CheckHealth();

            foreach (KeyValuePair<string, EndpointHealthResult> healthEndpoint in health.Endpoints)
                TestContext.WriteLine("Endpoint: {0}, Status: {1}", healthEndpoint.Key, healthEndpoint.Value.Description);

            Assert.That(health.Status, Is.EqualTo(BusHealthStatus.Healthy));
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
