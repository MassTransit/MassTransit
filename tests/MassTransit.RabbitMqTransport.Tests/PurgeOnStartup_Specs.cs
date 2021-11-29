namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using RabbitMQ.Client;
    using TestFramework.Messages;


    [TestFixture]
    public class Purging_a_receive_endpoint :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_remove_previously_sent_message()
        {
            Task<ConsumeContext<PingMessage>> pingHandled = null;
            Task<ConsumeContext<PongMessage>> pongHandled = null;

            var endpoint = await Bus.GetSendEndpoint(new Uri($"queue:{QueueName}"));

            for (var i = 0; i < 10; i++)
                await endpoint.Send(new PingMessage());

            var handle = Bus.ConnectReceiveEndpoint(QueueName, x =>
            {
                if (x is IRabbitMqReceiveEndpointConfigurator configurator)
                {
                    configurator.PrefetchCount = 1;
                    configurator.PurgeOnStartup = true;

                    pingHandled = Handled<PingMessage>(configurator);
                    pongHandled = Handled<PongMessage>(configurator);
                }
            });

            await handle.Ready;

            try
            {
                await endpoint.Send(new PongMessage());

                ConsumeContext<PongMessage> pinged = await pongHandled;

                Assert.That(pingHandled.Status, Is.EqualTo(TaskStatus.WaitingForActivation));
            }
            finally
            {
                await handle.StopAsync();
            }
        }

        const string QueueName = "connect_input_queue";

        protected override void OnCleanupVirtualHost(IModel model)
        {
            model.ExchangeDelete(QueueName);
            model.QueueDelete(QueueName);
        }
    }
}
