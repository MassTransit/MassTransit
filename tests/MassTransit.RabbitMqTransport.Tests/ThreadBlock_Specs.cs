namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture]
    public class When_the_bus_is_not_started
    {
        [Test]
        public async Task Should_not_leave_a_thread_hanging()
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                sbc.Host(new Uri("rabbitmq://localhost"), h =>
                {
                });
            });

            Assert.NotNull(bus);
        }

        [Test]
        public async Task Test()
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                sbc.Host(new Uri("rabbitmq://localhost"), h =>
                {
                });
            });

            Assert.NotNull(bus);

            await bus.StartAsync();
            await bus.StopAsync();
        }
    }
}
