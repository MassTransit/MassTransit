namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class PublishStop_Specs
    {
        [Test]
        [Explicit]
        public async Task Should_start_and_stop_async()
        {
            var queueUri = new Uri("rabbitmq://localhost/test/input_queue2");

            var rabbitMqHostSettings = queueUri.GetHostSettings();
            var receiveSettings = queueUri.GetReceiveSettings();

            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                sbc.Host(rabbitMqHostSettings);
                sbc.ReceiveEndpoint(receiveSettings.QueueName, ep =>
                {
                });
            });

            await bus.StartAsync();
            await bus.Publish(new DummyMessage { ID = 1 }).ConfigureAwait(false);
            await bus.StopAsync();
        }

        [Test]
        [Explicit]
        public async Task Should_start_and_stop_sync()
        {
            var queueUri = new Uri("rabbitmq://localhost/test/input_queue2");

            var rabbitMqHostSettings = queueUri.GetHostSettings();
            var receiveSettings = queueUri.GetReceiveSettings();

            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                BusTestFixture.ConfigureBusDiagnostics(sbc);

                sbc.Host(rabbitMqHostSettings);
                sbc.ReceiveEndpoint(receiveSettings.QueueName, ep =>
                {
                });
            });

            bus.Start();
            await bus.Publish(new DummyMessage { ID = 1 }).ConfigureAwait(false);
            bus.Stop();
        }


        class DummyMessage
        {
            public int ID { get; set; }
        }
    }
}
