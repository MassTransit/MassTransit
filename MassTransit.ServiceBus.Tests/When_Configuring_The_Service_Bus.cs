using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;

namespace MassTransit.ServiceBus.Tests
{
    [TestFixture]
    public class When_Configuring_The_Service_Bus
    {
        private MockRepository mocks;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
        }

        [TearDown]
        public void TearDown()
        {
            mocks = null;
        }

        [Test]
        public void A_MessageQueue_Transport_Should_Be_Usable()
        {
            string endpointName = @"msmq://localhost/test_servicebus";

            MessageQueueEndpoint defaultEndpoint = endpointName;

            ServiceBusSetupFixture.ValidateAndPurgeQueue(defaultEndpoint.QueueName);

            IServiceBus serviceBus = new ServiceBus(defaultEndpoint, mocks.CreateMock<ISubscriptionStorage>());

            string machineEndpointName = endpointName.Replace("localhost", Environment.MachineName.ToLowerInvariant());

            Assert.That(serviceBus.Endpoint.Uri.AbsoluteUri, Is.EqualTo(machineEndpointName));
        }
    }
}