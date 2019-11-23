namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class Sending_a_request_to_a_different_host
    {
        [Test]
        public async Task Should_be_able_to_respond()
        {
            IBusControl bus = Bus.Factory.CreateUsingRabbitMq(x =>
            {
                IRabbitMqHost host = x.Host("localhost", "test", h =>
                {
                });

                x.ReceiveEndpoint(host, "input_queue", e =>
                {
                    e.PurgeOnStartup = true;

                    e.Handler<PingMessage>(async context =>
                    {
                        await context.RespondAsync(new PongMessage(context.Message.CorrelationId));
                    });
                });
            });

            IBusControl clientBus = Bus.Factory.CreateUsingRabbitMq(x =>
            {
                IRabbitMqHost host = x.Host("127.0.0.1", "test", h =>
                {
                });

                x.AutoStart = true;
            });

            await bus.StartAsync();
            try
            {
                await clientBus.StartAsync();

                try
                {
                    var requestClient = clientBus.CreateRequestClient<PingMessage>(new Uri("rabbitmq://127.0.0.1/test/input_queue"));

                    var response = await requestClient.GetResponse<PongMessage>(new PingMessage());
                }
                finally
                {
                    await clientBus.StopAsync();
                }
            }
            finally
            {
                await bus.StopAsync();
            }
        }
    }
}
