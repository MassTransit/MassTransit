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
            MessageQueueEndpoint defaultEndpoint = @"msmq://localhost/test_servicebus";

            ValidateAndPurgeQueue(defaultEndpoint.QueueName);

            IServiceBus serviceBus = new ServiceBus(defaultEndpoint, mocks.CreateMock<ISubscriptionStorage>());

            Assert.That(serviceBus.Endpoint.Uri.AbsoluteUri, Is.EqualTo("msmq://localhost/test_servicebus"));
        }
    }
}