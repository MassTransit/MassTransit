using System;
using System.IO;
using System.Messaging;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using MassTransit.ServiceBus.Subscriptions;

namespace MassTransit.ServiceBus.Tests
{
    [TestFixture]
    public class MessageQueueEndpoint_MeetsCriteria
    {
        private MockRepository mocks;
        private ServiceBus _serviceBus;
        private IMessageQueueEndpoint _serviceBusEndPoint;
        private string queueName = @".\private$\test_servicebus";

        [SetUp]
        public void SetUp()
        {
            ServiceBusSetupFixture.ValidateAndPurgeQueue(queueName);
            mocks = new MockRepository();
            _serviceBusEndPoint = mocks.CreateMock<IMessageQueueEndpoint>();
        }
        [TearDown]
        public void TearDown()
        {
            mocks = null;
            _serviceBusEndPoint = null;
            _serviceBus = null;
        }

        [Test]
        public void The_Service_Bus_Should_Return_False_If_The_Message_Will_Not_Be_Handled()
        {
            using(mocks.Record())
            {
                Expect.Call(_serviceBusEndPoint.QueueName).Return(queueName);
                Expect.Call(_serviceBusEndPoint.QueueName).Return(queueName);
                Expect.Call(_serviceBusEndPoint.Uri).Return(new Uri("msmq://localhost/test_servicebus"));
            }
            using (mocks.Playback())
            {
                _serviceBus = new ServiceBus(_serviceBusEndPoint, new LocalSubscriptionCache());
                _serviceBus.Subscribe<PingMessage>(
                    delegate { },
                    delegate { return false; });

                IEnvelopeConsumer consumer = (IEnvelopeConsumer) _serviceBus;

                IEnvelope envelope = new Envelope(_serviceBusEndPoint, new PingMessage());

                Assert.That(consumer.IsHandled(envelope), Is.False);
            }
        }

        [Test]
        public void The_Service_Bus_Should_Return_True_If_The_Message_Will_Be_Handled()
        {
            using(mocks.Record())
            {
                Expect.Call(_serviceBusEndPoint.QueueName).Return(queueName);
                Expect.Call(_serviceBusEndPoint.QueueName).Return(queueName);
                Expect.Call(_serviceBusEndPoint.Uri).Return(new Uri("msmq://localhost/test_servicebus"));
            }
            using (mocks.Playback())
            {
                _serviceBus = new ServiceBus(_serviceBusEndPoint, new LocalSubscriptionCache());

                _serviceBus.Subscribe<PingMessage>(
                    delegate { },
                    delegate { return true; });

                IEnvelopeConsumer consumer = (IEnvelopeConsumer) _serviceBus;

                IEnvelope envelope = new Envelope(_serviceBusEndPoint, new PingMessage());

                Assert.That(consumer.IsHandled(envelope), Is.True);
            }
        }

        [Test]
        public void dru_test()
        {
            bool workDid = false;

            using (mocks.Record())
            {
                Expect.Call(_serviceBusEndPoint.QueueName).Return(queueName);
                Expect.Call(_serviceBusEndPoint.QueueName).Return(queueName);
                Expect.Call(_serviceBusEndPoint.Uri).Return(new Uri("msmq://localhost/test_servicebus"));
            }
            using (mocks.Playback())
            {
                _serviceBus = new ServiceBus(_serviceBusEndPoint, new LocalSubscriptionCache());

                _serviceBus.Subscribe<PingMessage>(
                    delegate { workDid = true; },
                    delegate { return true; });

                IEnvelopeConsumer consumer = (IEnvelopeConsumer)_serviceBus;

                IEnvelope envelope = new Envelope(_serviceBusEndPoint, new PingMessage());

                Assert.That(consumer.IsHandled(envelope), Is.True);
                consumer.Deliver(envelope);
                Assert.That(workDid, Is.True, "Lazy Test!");
            }
        }

        [Test]
        public void The_Service_Bus_Should_Return_True_If_The_Message_Will_Be_Handled_By_One_Of_multiple_handlers()
        {
            using(mocks.Record())
            {
                Expect.Call(_serviceBusEndPoint.QueueName).Return(queueName);
                Expect.Call(_serviceBusEndPoint.QueueName).Return(queueName);
                Expect.Call(_serviceBusEndPoint.Uri).Return(new Uri("msmq://localhost/test_servicebus"));
            }
            using (mocks.Playback())
            {
                _serviceBus = new ServiceBus(_serviceBusEndPoint, new LocalSubscriptionCache());

                _serviceBus.Subscribe<PingMessage>(
                    delegate { },
                    delegate { return false; });

                _serviceBus.Subscribe<PingMessage>(
                    delegate { },
                    delegate { return true; });

                IEnvelopeConsumer consumer = (IEnvelopeConsumer) _serviceBus;

                IEnvelope envelope = new Envelope(_serviceBusEndPoint, new PingMessage());

                Assert.That(consumer.IsHandled(envelope), Is.True);
            }
        }
        
        [Test]
        public void The_Service_Bus_Should_Return_False_If_The_Message_Will_Be_Handled_By_none_Of_the_handlers()
        {
            using(mocks.Record())
            {
                Expect.Call(_serviceBusEndPoint.QueueName).Return(queueName);
                Expect.Call(_serviceBusEndPoint.QueueName).Return(queueName);
                Expect.Call(_serviceBusEndPoint.Uri).Return(new Uri("msmq://localhost/test_servicebus"));
            }
            using (mocks.Playback())
            {
                _serviceBus = new ServiceBus(_serviceBusEndPoint, new LocalSubscriptionCache());

                _serviceBus.Subscribe<PingMessage>(
                    delegate { },
                    delegate { return false; });

                _serviceBus.Subscribe<PingMessage>(
                    delegate { },
                    delegate { return false; });

                IEnvelopeConsumer consumer = (IEnvelopeConsumer) _serviceBus;

                IEnvelope envelope = new Envelope(_serviceBusEndPoint, new PingMessage());

                Assert.That(consumer.IsHandled(envelope), Is.False);
            }
        }

        [Test]
        public void The_Message_Endpoint_Should_Check_If_The_Message_Will_Be_Handled()
        {
            IEndpoint mockReturnEndpoint = mocks.CreateMock<IEndpoint>();
            MessageQueueEndpoint endpoint = new MessageQueueEndpoint(@"msmq://localhost/test_endpoint");

            IEnvelopeConsumer consumer = mocks.CreateMock<IEnvelopeConsumer>();

            IMessageReceiver receiver = MessageReceiverFactory.Create(endpoint);
            receiver.Subscribe(consumer);

            PingMessage ping = new PingMessage();
            IEnvelope envelope = new Envelope(mockReturnEndpoint, ping);

            using(mocks.Record())
            {
                //I have no idea what changed - dds
                //Expect.Call(mockReturnEndpoint.Uri).Return(new Uri("msmq://localhost/test_endpoint"));
                Expect.Call(consumer.IsHandled(envelope)).Return(false).IgnoreArguments();
            }

            using (mocks.Playback())
            {
                Message queueMessage = EnvelopeMessageMapper.MapFrom(envelope);

                queueMessage.BodyStream.Seek(0, SeekOrigin.Begin);

                ((MessageQueueReceiver)receiver).ProcessMessage(queueMessage);
            }
        }
    }
}