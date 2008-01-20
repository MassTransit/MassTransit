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
        }

        #endregion

        private MockRepository mocks;

        [Test]
        public void A_message_receiver_should_be_creatable_for_a_MessageQueueEndpoint()
        {
            using (QueueTestContext qtc = new QueueTestContext())
            {
                IMessageReceiver receiver = qtc.ServiceBusEndPoint.Receiver;

                Assert.That(receiver, Is.Not.Null);
            }
        }

        [Test]
        public void A_message_sender_should_be_creatable_for_a_MessageQueueEndpoint()
        {
            using (QueueTestContext qtc = new QueueTestContext())
            {
                using (mocks.Record())
                {
                }
                using (mocks.Playback())
                {
                    IMessageSender sender = qtc.ServiceBusEndPoint.Sender;

                    Assert.That(sender, Is.Not.Null);
                }
            }
        }

        [Test, ExpectedException(typeof (EndpointException))]
        public void An_exception_should_be_thrown_when_creating_a_message_receiver_for_an_unknown_endpoint_type()
        {
            IEndpoint endpoint = mocks.CreateMock<IEndpoint>();

            IMessageReceiver receiver = endpoint.Receiver;
        }
    }
}