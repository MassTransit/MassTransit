namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture]
    public class Publishing_a_date_time_in_utc :
        AzureServiceBusTestFixture
    {
        [Test]
        public async Task Should_succeed()
        {
            await InputQueueSendEndpoint.Send(new DateTimeMessage {Time = DateTime.UtcNow});

            ConsumeContext<DateTimeMessage> consumeContext = await _handler;

            Assert.That(consumeContext.Message.Time.Kind, Is.EqualTo(DateTimeKind.Utc));
        }

        Task<ConsumeContext<DateTimeMessage>> _handler;

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumeTopology = false;

            _handler = Handled<DateTimeMessage>(configurator);
        }


        class DateTimeMessage
        {
            public DateTime Time { get; set; }
        }
    }
}
