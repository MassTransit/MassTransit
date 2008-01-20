using MassTransit.ServiceBus.Exceptions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;

namespace MassTransit.ServiceBus.Tests
{
    [TestFixture]
    public class When_working_with_an_endpoint
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            mockSenderFactory = mocks.CreateMock<IMessageSenderFactory>();
        }

        #endregion

        private MockRepository mocks;
        private IMessageSenderFactory mockSenderFactory;

        [Test]
        public void A_message_receiver_should_be_creatable_for_a_MessageQueueEndpoint()
        {
            using (QueueTestContext qtc = new QueueTestContext())
            {
                IMessageReceiver receiver = new MessageReceiverFactory().Using(qtc.ServiceBusEndPoint);

                Assert.That(receiver, Is.Not.Null);
            }
        }

        [Test]
        public void A_message_sender_should_be_creatable_for_a_MessageQueueEndpoint()
        {
            using (QueueTestContext qtc = new QueueTestContext())
            {
                IMessageSender mockSender = mocks.CreateMock<IMessageSender>();

                using (mocks.Record())
                {
                    Expect.Call(mockSenderFactory.Using(qtc.ServiceBusEndPoint)).Return(mockSender);
                }
                using (mocks.Playback())
                {
                    IMessageSender sender = mockSenderFactory.Using(qtc.ServiceBusEndPoint);

                    Assert.That(sender, Is.Not.Null);
                }
            }
        }

        [Test, ExpectedException(typeof (EndpointException))]
        public void An_exception_should_be_thrown_when_creating_a_message_receiver_for_an_unknown_endpoint_type()
        {
            IEndpoint endpoint = mocks.CreateMock<IEndpoint>();

            new MessageReceiverFactory().Using(endpoint);
        }
    }
}