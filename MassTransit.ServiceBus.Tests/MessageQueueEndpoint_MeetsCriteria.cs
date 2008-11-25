namespace MassTransit.Tests
{
    using System;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using ServiceBus;
    using ServiceBus.Internal;
    using ServiceBus.Tests.Messages;
    using ServiceBus.Transports;

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

            Assert.That(_serviceBus.Accept(_message), Is.True);


            _serviceBus.Dispatch(_message);

            Assert.That(workDid, Is.True, "Lazy Test!");
        }

        [Test]
        public void The_Service_Bus_Should_Return_False_If_The_Message_Will_Be_Handled_By_none_Of_the_handlers()
        {
            _serviceBus.Subscribe<PingMessage>(
                delegate { },
                delegate { return false; });

            _serviceBus.Subscribe<PingMessage>(
                delegate { },
                delegate { return false; });

            Assert.That(_serviceBus.Accept(_message), Is.False);
        }

        [Test]
        public void The_Service_Bus_Should_Return_False_If_The_Message_Will_Not_Be_Handled()
        {
            _serviceBus.Subscribe<PingMessage>(
                delegate { },
                delegate { return false; });

            Assert.That(_serviceBus.Accept(_message), Is.False);
        }

        [Test]
        public void The_Service_Bus_Should_Return_True_If_The_Message_Will_Be_Handled()
        {
            _serviceBus.Subscribe<PingMessage>(
                delegate { },
                delegate { return true; });

            Assert.That(_serviceBus.Accept(_message), Is.True);
        }

        [Test]
        public void The_Service_Bus_Should_Return_True_If_The_Message_Will_Be_Handled_By_One_Of_multiple_handlers()
        {
            _serviceBus.Subscribe<PingMessage>(
                delegate { },
                delegate { return false; });

            _serviceBus.Subscribe<PingMessage>(
                delegate { },
                delegate { return true; });


            Assert.That(_serviceBus.Accept(_message), Is.True);
        }
    }
}