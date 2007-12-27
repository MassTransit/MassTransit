using System.Messaging;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MassTransit.ServiceBus.Tests
{
    [TestFixture]
    public class When_Configuring_The_Service_Bus
    {
        [Test]
        public void A_MessageQueue_Transport_Should_Be_Usable()
        {
            string queuePath = @".\private$\test_servicebus";

            ValidateAndPurgeQueue(queuePath);

            MessageQueueEndpoint defaultEndpoint = queuePath;

            IServiceBus serviceBus = new ServiceBus(defaultEndpoint);

            Assert.That(serviceBus.DefaultEndpoint.Transport.Address, Is.EqualTo(queuePath));
        }

        private static void ValidateAndPurgeQueue(string queuePath)
        {
            try
            {
                MessageQueue.Create(queuePath);
            }
            catch (MessageQueueException ex)
            {
                if (ex.MessageQueueErrorCode != MessageQueueErrorCode.QueueExists)
                    throw;
            }

            MessageQueue queue = new MessageQueue(queuePath, QueueAccessMode.ReceiveAndAdmin);
            queue.Purge();
        }
    }
}