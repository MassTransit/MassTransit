namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using NUnit.Framework;


    [TestFixture]
    public class Subscribing_two_handlers_to_the_same_topic :
        AzureServiceBusTestFixture
    {
        [Test]
        public void Should_not_create_duplicate_subscriptions()
        {
        }

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            Handled<MessageA>(configurator);
            Handled<MessageA>(configurator);
        }


        public interface MessageA
        {
        }
    }
}
