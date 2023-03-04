namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework.Messages;
    using Testing;


    [TestFixture]
    public class Receive_single_message_with_tiny_lock_timeout :
        AzureServiceBusTestFixture
    {
        [Test]
        public async Task Should_receive_single_message()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());

            await InactivityTask;

            var count = await BusTestHarness.Consumed.SelectAsync<PingMessage>(x => x.Exception is null).Count();

            Assert.That(count, Is.EqualTo(1));
        }

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            configurator.MaxDeliveryCount = 5;
            configurator.LockDuration = TimeSpan.FromSeconds(5);
            configurator.MaxAutoRenewDuration = TimeSpan.FromSeconds(5);
            configurator.Consumer(() => new TestConsumer());
        }


        class TestConsumer :
            IConsumer<PingMessage>
        {
            public Task Consume(ConsumeContext<PingMessage> context)
            {
                return Task.Delay(TimeSpan.FromSeconds(10));
            }
        }
    }
}
