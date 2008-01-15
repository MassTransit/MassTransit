 using System;
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
        	string endpointName = @"msmq://localhost/test_servicebus";

			MessageQueueEndpoint defaultEndpoint = endpointName;

            ValidateAndPurgeQueue(defaultEndpoint.QueueName);

            IServiceBus serviceBus = new ServiceBus(defaultEndpoint, mocks.CreateMock<ISubscriptionStorage>());

        	string machineEndpointName = endpointName.Replace("localhost", Environment.MachineName.ToLowerInvariant());

            Assert.That(serviceBus.Endpoint.Uri.AbsoluteUri, Is.EqualTo(machineEndpointName));
        }
    }
}