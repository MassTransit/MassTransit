namespace MassTransit.RabbitMqTransport.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class Skipping_messages_should_not_crash_the_service :
        RabbitMqTestFixture
    {
        [Test]
        [Explicit]
        public async Task Should_properly_complete_without_dying()
        {
            await Task.WhenAll(Enumerable.Range(0, 1000).Select(n => Bus.Publish(new PingMessage())));

            await _pingsHandled;

            await Task.Delay(10000);
        }

        Task<ConsumeContext<PingMessage>> _pingsHandled;

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            configurator.Bind<PingMessage>();
        }

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("monitor", e =>
            {
                _pingsHandled = Handled<PingMessage>(e, 1000);
            });
        }
    }
}
