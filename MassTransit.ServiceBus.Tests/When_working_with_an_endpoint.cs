using System;
using System.Messaging;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MassTransit.ServiceBus.Tests
{
    using Internal;

    [TestFixture]
    public class When_working_with_an_endpoint
    {
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
                IMessageSender sender = qtc.ServiceBusEndPoint.Sender;

                Assert.That(sender, Is.Not.Null);
            }
        }

        [Test, ExpectedException(typeof (MessageQueueException))]
        public void An_exception_should_be_thrown_for_a_non_existant_queue()
        {
            MessageQueueEndpoint q = new MessageQueueEndpoint(new Uri("msmq://localhost/this_queue_does_not_exist"));

            q.Open(QueueAccessMode.ReceiveAndAdmin).GetAllMessages();
        }
    }
}