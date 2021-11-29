namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using RabbitMQ.Client;
    using TestFramework.Messages;


    [TestFixture]
    public class Binding_a_queue_on_send :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_be_allowed()
        {
            var serviceAddress = new UriBuilder(HostAddress + "/" + ServiceQueue) { Query = "bind=true" }.Uri;

            var endpoint = await Bus.GetSendEndpoint(serviceAddress);

            await endpoint.Send(new PingMessage());

            Task<ConsumeContext<PingMessage>> pingHandled = null;

            var handle = Bus.ConnectReceiveEndpoint(ServiceQueue, x =>
            {
                pingHandled = Handled<PingMessage>(x);
            });

            await handle.Ready;

            try
            {
                ConsumeContext<PingMessage> pinged = await pingHandled;
            }
            finally
            {
                await handle.StopAsync();
            }
        }

        const string ServiceQueue = "unbound-service";

        protected override void OnCleanupVirtualHost(IModel model)
        {
            model.ExchangeDelete(ServiceQueue);
            model.QueueDelete(ServiceQueue);
        }
    }
}
