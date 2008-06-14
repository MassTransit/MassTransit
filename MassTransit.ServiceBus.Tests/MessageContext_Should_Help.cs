namespace MassTransit.ServiceBus.Tests
{
    using MassTransit.ServiceBus.Internal;
    using NUnit.Framework;
    using Rhino.Mocks;

    [TestFixture]
    public class MessageContext_Should_Help :
        Specification
    {
        private IServiceBus mockBus;
        private IEndpoint mockBusEndpoint;
    	private IEndpoint mockEndpoint;

        private PingMessage requestMessage = new PingMessage();
        private PongMessage replyMessage = new PongMessage();

        protected override void Before_each()
        {
            mockBus = DynamicMock<IServiceBus>();
            mockBusEndpoint = DynamicMock<IEndpoint>();
            mockEndpoint = DynamicMock<IEndpoint>();
            DynamicMock<IEndpoint>();
            
        }
        protected override void After_each()
        {
            mockBus = null;
            mockBusEndpoint = null;
            mockEndpoint = null;
        }
        
        [Test, Ignore]
        public void With_Replies()
        {
            MessageContext<PingMessage> cxt = new MessageContext<PingMessage>(mockBus, requestMessage);

            using (Record())
            {
                Expect.Call(mockBus.Endpoint).Return(mockBusEndpoint);
            }

            using (Playback())
            {
                cxt.Reply(replyMessage);
            }
        }

        [Test]
        public void With_Handling_Later()
        {
            MessageContext<PingMessage> cxt = new MessageContext<PingMessage>(mockBus, requestMessage);

            using (Record())
            {
                Expect.Call(mockBus.Endpoint).Return(mockEndpoint);
            }

            using (Playback())
            {
                cxt.HandleMessageLater(replyMessage);
            }
        }

        [Test]
        public void With_Poison_Letters()
        {
            MessageContext<PingMessage> cxt = new MessageContext<PingMessage>(mockBus, requestMessage);

            using (Record())
            {
                //   Expect.Call(mockBus.PoisonEndpoint).Return(mockPoisonEndpoint);
            }

            using (Playback())
            {
                cxt.MarkPoison();
            }
        }

        [Test]
        public void With_Poison_Letter()
        {
            MessageContext<PingMessage> cxt = new MessageContext<PingMessage>(mockBus, requestMessage);

            using (Record())
            {
                //Expect.Call(mockBus.PoisonEndpoint).Return(mockPoisonEndpoint);
            }

            using (Playback())
            {
                cxt.MarkPoison(cxt.Message);
            }
        }
    }
}