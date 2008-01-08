using System;
using System.IO;
using System.Messaging;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;

namespace MassTransit.ServiceBus.Tests
{
    [TestFixture]
    public class MessageQueueEndpoint_MeetsCriteria
        : ServiceBusSetupFixture
    {
        MockRepository mocks = new MockRepository();

        [Test]
        public void The_Service_Bus_Should_Return_False_If_The_Message_Will_Not_Be_Handled()
        {
            _serviceBus.Subscribe<PingMessage>(
                delegate { },
                delegate { return false; });

            IEnvelopeConsumer consumer = (IEnvelopeConsumer) _serviceBus;

            IEnvelope envelope = new Envelope(_serviceBusEndPoint, new PingMessage());

            Assert.That(consumer.MeetsCriteria(envelope), Is.False);
        }

        [Test]
        public void The_Service_Bus_Should_Return_True_If_The_Message_Will_Be_Handled()
        {
            _serviceBus.Subscribe<PingMessage>(
                delegate { },
                delegate { return true; });

            IEnvelopeConsumer consumer = (IEnvelopeConsumer) _serviceBus;

            IEnvelope envelope = new Envelope(_serviceBusEndPoint, new PingMessage());

            Assert.That(consumer.MeetsCriteria(envelope), Is.True);
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

            IEnvelopeConsumer consumer = (IEnvelopeConsumer) _serviceBus;

            IEnvelope envelope = new Envelope(_serviceBusEndPoint, new PingMessage());

            Assert.That(consumer.MeetsCriteria(envelope), Is.True);
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

            IEnvelopeConsumer consumer = (IEnvelopeConsumer)_serviceBus;

            IEnvelope envelope = new Envelope(_serviceBusEndPoint, new PingMessage());

            Assert.That(consumer.MeetsCriteria(envelope), Is.False);
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
                Expect.Call(mockReturnEndpoint.Uri).Return(new Uri("msmq://localhost/test_endpoint"));
                Expect.Call(consumer.MeetsCriteria(envelope)).Return(false).IgnoreArguments();
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