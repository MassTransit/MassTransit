namespace MassTransit.ServiceBus.Tests
{
    using System.Messaging;
    using Messages;
    using NUnit.Framework;
    using Rhino.Mocks;

    [TestFixture]
    public class MessageQueueEndpoint_MeetsCriteria
    {
        private MockRepository mocks;
        private MessageQueueEndpoint _endpoint;
        private IEnvelopeConsumer _consumer;
        private PingMessage _msg;
        private Message msmqMsg = new Message();

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            _msg = new PingMessage();
            _endpoint = new MessageQueueEndpoint(@".\private$\test");
            _consumer = mocks.CreateMock<IEnvelopeConsumer>();
            _endpoint.SerializeMessages(msmqMsg.BodyStream, new IMessage[] {_msg});
        }

        [TearDown]
        public void TearDown()
        {
            mocks = null;
            _endpoint = null;
            _consumer = null;
        }

        [Test]
        public void Should_Hit_The_Second_Delegate()
        {
            _endpoint.Subscribe(_consumer);

            _endpoint.ProcessMessage(msmqMsg);
        }
    }
}