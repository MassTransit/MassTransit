 using System.Messaging;
 using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MassTransit.ServiceBus.Tests
{
    using Rhino.Mocks;

    [TestFixture]
    public class When_Configuring_The_Service_Bus :
        ServiceBusSetupFixture
    {
        private MockRepository mocks = new MockRepository();


        [Test]
        public void A_MessageQueue_Transport_Should_Be_Usable()
        {
            string queuePath = @".\private$\test_servicebus";

            queuePath = Util.MsmqUtilities.NormalizeQueueName(new MessageQueue(queuePath));

            ValidateAndPurgeQueue(queuePath);

            MessageQueueEndpoint defaultEndpoint = new MessageQueueEndpoint(queuePath);

            IServiceBus serviceBus = new ServiceBus(defaultEndpoint, mocks.CreateMock<ISubscriptionStorage>());

            Assert.That(serviceBus.Endpoint.Address, Is.EqualTo(queuePath));
        }
    }
}