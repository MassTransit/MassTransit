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
            MessageContext<RequestMessage> cxt = new MessageContext<RequestMessage>(mockEnvelope, requestMessage, mockBus);
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
    }

    public class RequestMessage : IMessage { }
    public class ReplyMessage : IMessage { }
}