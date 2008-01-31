using NUnit.Framework;
using Rhino.Mocks;

namespace MassTransit.ServiceBus.Tests
{
    using Internal;
    using Util;

    [TestFixture]
    public class MessageContext_Should_Help
    {
        private MockRepository mocks;
        private IServiceBus mockBus;
        private IEndpoint mockBusEndpoint;
        private IEnvelope mockEnvelope;
        private IEndpoint mockPoisonEndpoint;
        private IMessageQueueEndpoint mockEndpoint;

        private PingMessage requestMessage = new PingMessage();
        private PongMessage replyMessage = new PongMessage();
        private IMessageSender mockMessageSender;

        #region SetUp / TearDown
        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            mockBus = mocks.CreateMock<IServiceBus>();
            mockBusEndpoint = mocks.CreateMock<IEndpoint>();
            mockEnvelope = mocks.CreateMock<IEnvelope>();
            mockEndpoint = mocks.CreateMock<IMessageQueueEndpoint>();
            mockPoisonEndpoint = mocks.CreateMock<IEndpoint>();
            mockMessageSender = mocks.CreateMock<IMessageSender>();
        }

        [TearDown]
        public void TearDown()
        {
            mocks = null;
            mockBus = null;
            mockBusEndpoint = null;
            mockEnvelope = null;
            mockEndpoint = null;
            mockPoisonEndpoint = null;
            mockMessageSender = null;
        }
        #endregion

        [Test]
        public void With_Replies()
        {
            MessageContext<PingMessage> cxt = new MessageContext<PingMessage>(mockBus, mockEnvelope, requestMessage);

            using (mocks.Record())
            {
                Expect.Call(mockEnvelope.ReturnEndpoint).Return(mockEndpoint);
                Expect.Call(mockBus.Endpoint).Return(mockBusEndpoint);
                Expect.Call(mockEnvelope.Id).Return(MessageId.Empty);
                Expect.Call(mockEndpoint.Sender).Return(mockMessageSender).Repeat.Any();

                Expect.Call(delegate { mockMessageSender.Send(null); }).IgnoreArguments(); //ignoring arguments because we create a new envelope in the method

            }

            using (mocks.Playback())
            {
                cxt.Reply(replyMessage);
            }
        }

        [Test]
        public void With_Handling_Later()
        {

            MessageContext<PingMessage> cxt = new MessageContext<PingMessage>(mockBus, mockEnvelope, requestMessage);

            using (mocks.Record())
            {
                Expect.Call(mockBus.Endpoint).Return(mockEndpoint);
                Expect.Call(mockEndpoint.Sender).Return(mockMessageSender);
                Expect.Call(delegate { mockMessageSender.Send(null); }).IgnoreArguments(); //ignoring arguments because we create a new envelope in the method
            }

            using (mocks.Playback())
            {
                cxt.HandleMessagesLater(replyMessage);
            }
        }

        [Test]
        public void With_Poison_Letters()
        {
            MessageContext<PingMessage> cxt = new MessageContext<PingMessage>(mockBus, mockEnvelope, requestMessage);

            using (mocks.Record())
            {
                Expect.Call(mockBus.PoisonEndpoint).Return(mockPoisonEndpoint);
                Expect.Call(mockPoisonEndpoint.Sender).Return(mockMessageSender);
                Expect.Call(delegate { mockMessageSender.Send(null); }).IgnoreArguments(); //ignoring arguments because we create a new envelope in the method
            }

            using (mocks.Playback())
            {
                cxt.MarkPoison();
            }
        }

        [Test]
        public void With_Poison_Letter()
        {
            MessageContext<PingMessage> cxt = new MessageContext<PingMessage>(mockBus, mockEnvelope, requestMessage);

            using (mocks.Record())
            {
                Expect.Call(mockEnvelope.Clone()).Return(mockEnvelope);
                mockEnvelope.Messages = new IMessage[] { requestMessage };
                Expect.Call(mockBus.PoisonEndpoint).Return(mockPoisonEndpoint);
                Expect.Call(mockPoisonEndpoint.Sender).Return(mockMessageSender);
                Expect.Call(delegate { mockMessageSender.Send(null); }).IgnoreArguments(); //ignoring arguments because we create a new envelope in the method
            }

            using (mocks.Playback())
            {
                cxt.MarkPoison(cxt.Message);
            }
        }
    }
}