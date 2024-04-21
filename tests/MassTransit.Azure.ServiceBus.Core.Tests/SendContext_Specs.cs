namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class Sending_with_a_send_context :
        AzureServiceBusTestFixture
    {
        [Test]
        [Explicit]
        public async Task Should_succeed()
        {
            await InputQueueSendEndpoint.Send(new PingMessage(), context => context.SetScheduledEnqueueTime(TimeSpan.FromSeconds(10)));

            var timer = Stopwatch.StartNew();

            await _handler;

            timer.Stop();

            Assert.That(timer.Elapsed, Is.GreaterThanOrEqualTo(TimeSpan.FromSeconds(10)));
        }

        Task<ConsumeContext<PingMessage>> _handler;

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            _handler = Handled<PingMessage>(configurator);
        }
    }
}
