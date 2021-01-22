namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture]
    public class When_insufficient_permissions_are_specified
    {
        [Test]
        [Explicit]
        public async Task Should_cleanup_when_permissions_are_lame()
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(x =>
            {
                x.Host(new Uri("rabbitmq://localhost/test"), h =>
                {
                    h.Username("unguest");
                    h.Password("guest");
                });
            });

            Assert.That(async () =>
            {
                var handle = await busControl.StartAsync();
                try
                {
                    Console.WriteLine("Waiting for connection...");

                    await handle.Ready;
                }
                finally
                {
                    await handle.StopAsync();
                }
            }, Throws.TypeOf<RabbitMqConnectionException>());

            await Task.Delay(15000);
        }
    }
}
