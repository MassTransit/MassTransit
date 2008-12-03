namespace MassTransit.Tests
{
    using MassTransit.Internal;
    using NUnit.Framework;
    using Rhino.Mocks;
    
    using Tests.Messages;

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
            IServiceBus bs = MockRepository.GenerateMock<IServiceBus>();
            IEndpoint pep = MockRepository.GenerateMock<IEndpoint>();
            MessageContext<PingMessage> cxt = new MessageContext<PingMessage>(bs, requestMessage);

            bs.Expect(x => x.PoisonEndpoint).Return(pep);
            pep.Expect(x => x.Send(requestMessage));


            cxt.MarkPoisonous();

            bs.VerifyAllExpectations();
            pep.VerifyAllExpectations();

        }

    }
}