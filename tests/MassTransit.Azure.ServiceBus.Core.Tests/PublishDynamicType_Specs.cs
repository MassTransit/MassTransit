namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class Publishing_an_anonymous_type :
        AzureServiceBusTestFixture
    {
        [Test]
        public async Task Should_throw_a_proper_exception()
        {
            Assert.That(async () => await Bus.Publish(new { Value = "Name" }), Throws.TypeOf<ArgumentException>().With.Message.Contain("anonymous").IgnoreCase);
        }

        Task<ConsumeContext<PingMessage>> _handler;

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            _handler = Handled<PingMessage>(configurator);
        }
    }
}
