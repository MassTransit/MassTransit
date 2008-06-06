namespace MassTransit.ServiceBus.Tests
{
    using Internal;
    using NUnit.Framework;
    using Rhino.Mocks;

    [TestFixture]
    public class MessageContext_Should_Help :
        Specification
    {
        private IServiceBus mockBus;
        private IEndpoint mockBusEndpoint;
        private IEnvelope mockEnvelope;
        private IEndpoint mockPoisonEndpoint;
        private IEndpoint mockEndpoint;

        private PingMessage requestMessage = new PingMessage();
        private PongMessage replyMessage = new PongMessage();

        #region SetUp / TearDown

        protected override void Before_each()
        {
            mockBus = DynamicMock<IServiceBus>();
            mockBusEndpoint = DynamicMock<IEndpoint>();
            mockEnvelope = DynamicMock<IEnvelope>();
            mockEndpoint = DynamicMock<IEndpoint>();
            mockPoisonEndpoint = DynamicMock<IEndpoint>();
            
        }
        protected override void After_each()
        {
            mockBus = null;
            mockBusEndpoint = null;
            mockEnvelope = null;
            mockEndpoint = null;
            mockPoisonEndpoint = null;
            
        }
        
        #endregion

        [Test, Ignore]
        public void With_Replies()
        {
            MessageContext<PingMessage> cxt = new MessageContext<PingMessage>(mockBus, mockEnvelope, requestMessage);

            using (Record())
            {
                Expect.Call(mockEnvelope.ReturnEndpoint).Return(mockEndpoint);
                Expect.Call(mockBus.Endpoint).Return(mockBusEndpoint);
                Expect.Call(mockEnvelope.Id).Return(null);
            }

            using (Playback())
            {
                cxt.Reply(replyMessage);
            }
        }

        [Test]
        public void With_Handling_Later()
        {
            MessageContext<PingMessage> cxt = new MessageContext<PingMessage>(mockBus, mockEnvelope, requestMessage);

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
            MessageContext<PingMessage> cxt = new MessageContext<PingMessage>(mockBus, mockEnvelope, requestMessage);

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
            MessageContext<PingMessage> cxt = new MessageContext<PingMessage>(mockBus, mockEnvelope, requestMessage);

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