using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using MassTransit.ServiceBus.Subscriptions;
using Rhino.Mocks;

namespace MassTransit.ServiceBus.Tests
{
    [TestFixture]
    public class When_A_Message_Is_Received_On_The_Service_Bus
    {
        private MockRepository mocks;
        private ServiceBus _serviceBus;
        private IMessageQueueEndpoint _serviceBusEndPoint;
        private IMessageReceiverFactory mockReceiverFactory;
        private IMessageSenderFactory mockSenderFactory;
        private IMessageReceiver mockReceiver;
        private string queueName = @".\private$\test_servicebus";
        [SetUp]
        public void SetUp()
        {
            ServiceBusSetupFixture.ValidateAndPurgeQueue(queueName);
            mocks = new MockRepository();
            _serviceBusEndPoint = mocks.CreateMock<IMessageQueueEndpoint>();
            mockReceiverFactory = mocks.CreateMock<IMessageReceiverFactory>();
            mockSenderFactory = mocks.CreateMock<IMessageSenderFactory>();
            mockReceiver = mocks.CreateMock<IMessageReceiver>();
        }

        [TearDown]
        public void TearDown()
        {
            mocks = null;
            _serviceBus = null;
            _serviceBusEndPoint = null;
        }

        [Test]
        public void An_Event_Handler_Should_Be_Called()
        {
            using(mocks.Record())
            {
                Expect.Call(mockReceiverFactory.Using(_serviceBusEndPoint)).Return(mockReceiver);
                Expect.Call(delegate { mockReceiver.Subscribe(null); }).IgnoreArguments();
                Expect.Call(_serviceBusEndPoint.Uri).Return(new Uri("msmq://localhost/test_servicebus")).Repeat.Any(); //stupid log4net
            }
            using (mocks.Playback())
            {
                _serviceBus = new ServiceBus(_serviceBusEndPoint, new LocalSubscriptionCache(), mockSenderFactory, mockReceiverFactory);

                bool _received = false;

                MessageReceivedCallback<PingMessage> handler = delegate
                                                                   {
                                                                       _received = true;
                                                                   };

                _serviceBus.Subscribe(handler);

                IEnvelope envelope = new Envelope(_serviceBusEndPoint, new PingMessage());
                _serviceBus.Deliver(envelope);

                Assert.That(_received, Is.True);
            }
        }

        [Test]
        public void What_Happens_If_No_Subscriptions()
        {
            using(mocks.Record())
            {
                Expect.Call(_serviceBusEndPoint.Uri).Return(new Uri("msmq://localhost/test_servicebus")).Repeat.Any(); //stupid log4net
            }
            using (mocks.Playback())
            {
                _serviceBus = new ServiceBus(_serviceBusEndPoint, new LocalSubscriptionCache(), mockSenderFactory, mockReceiverFactory);

                bool _received = false;

                IEnvelope envelope = new Envelope(_serviceBusEndPoint, new PingMessage());
                _serviceBus.Deliver(envelope);

                Assert.That(_received, Is.False);
            }
        }
    }
}
