using MassTransit.ServiceBus.Tests.Messages;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

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

        //private MockRepository mocks;
        //private MessageQueueEndpoint _endpoint;
        //private IEnvelopeConsumer _consumer;
        //private PingMessage _msg;
        //private Message msmqMsg = new Message();

        //[SetUp]
        //public void SetUp()
        //{
        //    mocks = new MockRepository();
        //    _msg = new PingMessage();
        //    _endpoint = new MessageQueueEndpoint(@".\private$\test");
        //    _consumer = mocks.CreateMock<IEnvelopeConsumer>();
        //    _endpoint.SerializeMessages(msmqMsg.BodyStream, new IMessage[] {_msg});
        //}

        //[TearDown]
        //public void TearDown()
        //{
        //    mocks = null;
        //    _endpoint = null;
        //    _consumer = null;
        //}

        //[Test]
        //public void Should_Hit_The_Second_Delegate()
        //{
        //    _endpoint.Subscribe(_consumer);

        //    _endpoint.ProcessMessage(msmqMsg);
        //}
    }
}