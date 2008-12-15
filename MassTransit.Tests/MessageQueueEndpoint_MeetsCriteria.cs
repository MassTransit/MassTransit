namespace MassTransit.Tests
{
    using System;
    using MassTransit.Internal;
    using Messages;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using Transports;

    [TestFixture]
    public class MessageQueueEndpoint_MeetsCriteria :
        Specification
    {
        private ServiceBus _serviceBus;
        private IEndpoint _mockServiceBusEndPoint;
        private readonly PingMessage _message = new PingMessage();

        private Uri _busListenUri = new Uri("msmq://localhost/bus_listen");
        private EndpointResolver _endpointResolver;

        protected override void Before_each()
        {
            EndpointResolver.AddTransport(typeof (LoopbackEndpoint));

            _endpointResolver = new EndpointResolver();
            _mockServiceBusEndPoint = _endpointResolver.Resolve(new Uri("loopback://localhost/test"));
            _serviceBus = new ServiceBus(_mockServiceBusEndPoint, DynamicMock<IObjectBuilder>());
            ReplayAll();
        }

        protected override void After_each()
        {
            _mockServiceBusEndPoint = null;
            _serviceBus = null;
        }


        [Test]
        public void Subscring_to_an_endpoint_should_accept_and_dispatch_messages()
        {
            bool workDid = false;

            _serviceBus.Subscribe<PingMessage>(
                delegate { workDid = true; },
                delegate { return true; });

            _serviceBus.Dispatch(_message);

            Assert.That(workDid, Is.True, "Lazy Test!");
        }
    }
}