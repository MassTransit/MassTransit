namespace MassTransit.ServiceBus.Tests
{
    using NUnit.Framework;
    using Rhino.Mocks;

    [TestFixture]
    public class MessageContext_Should_Help
    {
        private MockRepository mocks;
        private IServiceBus mockBus;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            mockBus = mocks.CreateMock<IServiceBus>();
        }

        [TearDown]
        public void TearDown()
        {
            mocks = null;
            mockBus = null;
        }

        [Test]
        public void With_Replies()
        {
            IEnvelope mockEnvelope = mocks.CreateMock<IEnvelope>();
            IEndpoint mockEndpoint = mocks.CreateMock<IEndpoint>();
            RequestMessage requestMessage = new RequestMessage();
            MessageContext<RequestMessage> cxt = new MessageContext<RequestMessage>(mockBus, mockEnvelope, requestMessage);
            ReplyMessage replyMessage = new ReplyMessage();
            IMessage[] messages = new IMessage[1]{replyMessage};

            using(mocks.Record())
            {
                Expect.Call(mockEnvelope.ReturnTo).Return(mockEndpoint);
                mockBus.Send(mockEndpoint, messages);
            }

            using(mocks.Playback())
            {
                cxt.Reply(replyMessage);
            }
        }

        [Test]
        public void With_Handling_Later()
        {
            IEnvelope mockEnvelope = mocks.CreateMock<IEnvelope>();
            IEndpoint mockEndpoint = mocks.CreateMock<IEndpoint>();
            RequestMessage requestMessage = new RequestMessage();
            MessageContext<RequestMessage> cxt = new MessageContext<RequestMessage>(mockBus, mockEnvelope, requestMessage);
            ReplyMessage replyMessage = new ReplyMessage();
            IMessage[] messages = new IMessage[1] { replyMessage };

            using (mocks.Record())
            {
                Expect.Call(mockBus.Endpoint).Return(mockEndpoint);
                mockBus.Send(mockEndpoint, messages);
            }

            using (mocks.Playback())
            {
                cxt.HandleMessagesLater(replyMessage);
            }
        }

        [Test]
        public void With_Poison_Letters()
        {
            IEnvelope mockEnvelope = mocks.CreateMock<IEnvelope>();
            IEndpoint mockEndpoint = mocks.CreateMock<IEndpoint>();
            IEndpoint mockPoisonEndpoint = mocks.CreateMock<IEndpoint>();
            RequestMessage requestMessage = new RequestMessage();
            MessageContext<RequestMessage> cxt = new MessageContext<RequestMessage>(mockBus, mockEnvelope, requestMessage);

            using (mocks.Record())
            {
                Expect.Call(mockBus.Endpoint).Return(mockEndpoint);
                Expect.Call(mockEndpoint.PoisonEndpoint).Return(mockPoisonEndpoint);
                mockPoisonEndpoint.Send(mockEnvelope);
            }

            using (mocks.Playback())
            {
                cxt.MarkPoison();
            }
        }

        [Test]
        public void With_Poison_Letter()
        {
            IEnvelope mockEnvelope = mocks.CreateMock<IEnvelope>();
            IEndpoint mockEndpoint = mocks.CreateMock<IEndpoint>();
            IEndpoint mockPoisonEndpoint = mocks.CreateMock<IEndpoint>();
            RequestMessage requestMessage = new RequestMessage();
            MessageContext<RequestMessage> cxt = new MessageContext<RequestMessage>(mockBus, mockEnvelope, requestMessage);

            using (mocks.Record())
            {
                Expect.Call(mockEnvelope.Clone()).Return(mockEnvelope);
                mockEnvelope.Messages = new IMessage[] { requestMessage };
                Expect.Call(mockBus.Endpoint).Return(mockEndpoint);
                Expect.Call(mockEndpoint.PoisonEndpoint).Return(mockPoisonEndpoint);
                mockPoisonEndpoint.Send(mockEnvelope);
            }

            using (mocks.Playback())
            {
                cxt.MarkPoison(cxt.Message);
            }
        }
    }

    public class RequestMessage : IMessage { }
    public class ReplyMessage : IMessage { }
}