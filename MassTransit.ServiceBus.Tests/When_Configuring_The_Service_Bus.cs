 using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MassTransit.ServiceBus.Tests
{
    [TestFixture]
    public class When_Configuring_The_Service_Bus :
        ServiceBusSetupFixture
    {
        [Test]
        public void A_MessageQueue_Transport_Should_Be_Usable()
        {
            string queuePath = @".\private$\test_servicebus";

            ValidateAndPurgeQueue(queuePath);

            MessageQueueEndpoint defaultEndpoint = new MessageQueueEndpoint(queuePath);

            IServiceBus serviceBus = new ServiceBus(defaultEndpoint);

            Assert.That(serviceBus.Endpoint.Address, Is.EqualTo(queuePath));
        }
    }
}