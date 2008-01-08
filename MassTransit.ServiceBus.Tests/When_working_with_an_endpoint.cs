using MassTransit.ServiceBus.Exceptions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;

namespace MassTransit.ServiceBus.Tests
{
    [TestFixture]
    public class When_working_with_an_endpoint
    {
        [Test]
        public void A_message_sender_should_be_creatable_for_a_MessageQueueEndpoint()
        {
            using (QueueTestContext qtc = new QueueTestContext())
            {
                IMessageSender sender = MessageSenderFactory.Create(qtc.ServiceBusEndPoint);

                Assert.That(sender, Is.Not.Null);
            }
        }

        [Test, ExpectedException(typeof(EndpointException))]
        public void An_exception_should_be_thrown_when_creating_a_message_sender_for_an_unknown_endpoint_type()
        {
            MockRepository mocks = new MockRepository();

            IEndpoint endpoint = mocks.CreateMock<IEndpoint>();

            MessageSenderFactory.Create(endpoint);
        }

        [Test]
        public void A_message_receiver_should_be_creatable_for_a_MessageQueueEndpoint()
        {
            using (QueueTestContext qtc = new QueueTestContext())
            {
                IMessageReceiver receiver = MessageReceiverFactory.Create(qtc.ServiceBusEndPoint);

                Assert.That(receiver, Is.Not.Null);
            }
        }

        [Test, ExpectedException(typeof(EndpointException))]
        public void An_exception_should_be_thrown_when_creating_a_message_receiver_for_an_unknown_endpoint_type()
        {
            MockRepository mocks = new MockRepository();

            IEndpoint endpoint = mocks.CreateMock<IEndpoint>();

            MessageReceiverFactory.Create(endpoint);
        }

    }
}