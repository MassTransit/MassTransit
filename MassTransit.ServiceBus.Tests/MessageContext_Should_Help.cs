namespace MassTransit.ServiceBus.Tests
{
    using System;
    using NUnit.Framework;
    using Rhino.Mocks;
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

        private string queueName = "";

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
        }
        #endregion

        [Test]
        [Ignore("MessageSender.Using is killing the mock test approach")]
        public void With_Replies()
        {
            //TODO: this test is now hitting the machine
            MessageContext<PingMessage> cxt = new MessageContext<PingMessage>(mockBus, mockEnvelope, requestMessage);

            using (mocks.Record())
            {
                Expect.Call(mockEnvelope.ReturnEndpoint).Return(mockEndpoint);
                Expect.Call(mockBus.Endpoint).Return(mockBusEndpoint);
                Expect.Call(mockEnvelope.Id).Return("");
                Expect.Call(mockEndpoint.QueueName).Return(@".\private$\test_client");
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
            
            IMessage[] messages = new IMessage[1] { replyMessage };

            using (mocks.Record())
            {
                //TODO: Can we better hide the mapper?
                Expect.Call(mockBus.Endpoint).Return(mockEndpoint);
                Expect.Call(mockEndpoint.QueueName).Return(@".\private$\test_client");
                Expect.Call(mockEnvelope.Messages).Return(null);
                Expect.Call(mockEnvelope.ReturnEndpoint).Return(mockEndpoint);
                Expect.Call(mockEndpoint.QueueName).Return(@".\private$\test_client");
                Expect.Call(mockEnvelope.TimeToBeReceived).Return(new TimeSpan(1, 0, 0));
                Expect.Call(mockEnvelope.TimeToBeReceived).Return(new TimeSpan(1, 0, 0));
                Expect.Call(mockEnvelope.Label).Return("bob");
                Expect.Call(mockEnvelope.Label).Return("bob");
                Expect.Call(mockEnvelope.Recoverable).Return(true);
                Expect.Call(mockEnvelope.CorrelationId).Return(MessageId.Empty);
                mockEnvelope.Id = "";
                LastCall.IgnoreArguments();
            }

            using (mocks.Playback())
            {
                cxt.HandleMessagesLater(replyMessage);
            }
        }

        [Test]
        [Ignore("MessageSender.Using is killing the mock test approach")]
        public void With_Poison_Letters()
        {
            MessageContext<PingMessage> cxt = new MessageContext<PingMessage>(mockBus, mockEnvelope, requestMessage);

            using (mocks.Record())
            {
                Expect.Call(mockEnvelope.Id).PropertyBehavior(); //stupid log4net
                Expect.Call(mockEnvelope.Messages).PropertyBehavior(); //stupid log4net
                Expect.Call(mockBus.Endpoint).Return(mockEndpoint);
                Expect.Call(mockBus.PoisonEndpoint).Return(mockPoisonEndpoint);
                Expect.Call(mockPoisonEndpoint.Uri).Return(new Uri("msmq://localhost/test_servicebus_poison"));
            }

            using (mocks.Playback())
            {
                cxt.MarkPoison();
            }
        }

        [Test]
        [Ignore("MessageSender.Using is killing the mock test approach")]
        public void With_Poison_Letter()
        {
            MessageContext<PingMessage> cxt = new MessageContext<PingMessage>(mockBus, mockEnvelope, requestMessage);

            using (mocks.Record())
            {
                Expect.Call(mockEnvelope.Id).PropertyBehavior(); //stupid log4net
                Expect.Call(mockEnvelope.Messages).PropertyBehavior(); //stupid log4net
                Expect.Call(mockEnvelope.Clone()).Return(mockEnvelope);
                mockEnvelope.Messages = new IMessage[] { requestMessage };
                Expect.Call(mockBus.Endpoint).Return(mockEndpoint);
                Expect.Call(mockBus.PoisonEndpoint).Return(mockPoisonEndpoint);
                Expect.Call(mockPoisonEndpoint.Uri).Return(new Uri("msmq://localhost/test_servicebus_poison"));
            }

            using (mocks.Playback())
            {
                cxt.MarkPoison(cxt.Message);
            }
        }
    }
}