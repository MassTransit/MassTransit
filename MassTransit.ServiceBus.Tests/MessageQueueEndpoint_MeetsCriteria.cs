using System.IO;
using System.Messaging;
using MassTransit.ServiceBus.Tests.Messages;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;

namespace MassTransit.ServiceBus.Tests
{
    [TestFixture]
    public class MessageQueueEndpoint_MeetsCriteria
        : ServiceBusSetupFixture
    {
        [Test]
        public void The_Service_Bus_Should_Return_False_If_The_Message_Will_Not_Be_Handled()
        {
            _serviceBus.Subscribe<PingMessage>(
                delegate { },
                delegate { return false; });

            IEnvelopeConsumer consumer = (IEnvelopeConsumer) _serviceBus;

            IEnvelope envelope = new Envelope(new PingMessage());

            Assert.That(consumer.MeetsCriteria(envelope), Is.False);
        }

        [Test]
        public void The_Service_Bus_Should_Return_True_If_The_Message_Will_Be_Handled()
        {
            _serviceBus.Subscribe<PingMessage>(
                delegate { },
                delegate { return true; });

            IEnvelopeConsumer consumer = (IEnvelopeConsumer) _serviceBus;

            IEnvelope envelope = new Envelope(new PingMessage());

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

            IEnvelope envelope = new Envelope(new PingMessage());

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

            IEnvelope envelope = new Envelope(new PingMessage());

            Assert.That(consumer.MeetsCriteria(envelope), Is.False);
        }

        [Test]
        public void The_Message_Endpoint_Should_Check_If_The_Message_Will_Be_Handled()
        {
            MessageQueueEndpoint endpoint = MessageQueueEndpoint.Open(@".\private$\test_endpoint");

            MockRepository mocks = new MockRepository();

            IEnvelopeConsumer consumer = mocks.CreateMock<IEnvelopeConsumer>();

            endpoint.Subscribe(consumer);

            IEnvelope envelope = new Envelope();

            using(mocks.Record())
            {
                consumer.MeetsCriteria(envelope);
                LastCall.IgnoreArguments();
                LastCall.Return(false);
            }

            using (mocks.Playback())
            {
                Message queueMessage = new Message();

                PingMessage ping = new PingMessage();

                endpoint.SerializeMessages(queueMessage.BodyStream, new IMessage[] {ping});
                queueMessage.BodyStream.Seek(0, SeekOrigin.Begin);

                endpoint.ProcessMessage(queueMessage);
            }
        }
    }
}