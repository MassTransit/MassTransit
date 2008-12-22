namespace MassTransit.Transports.Msmq.Tests
{
    using System;
    using System.Messaging;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Transactions;
    using MassTransit.Tests;

    public static class MsmqEndpoint_Extensions
    {
        public static void Purge(this MsmqEndpoint ep)
        {
            MessageQueue queue = new MessageQueue(ep.QueuePath, QueueAccessMode.ReceiveAndAdmin);
            queue.Purge();
        }


        public static void VerifyMessageInQueue<T>(this MsmqEndpoint ep)
        {
            using (MessageQueue mq = new MessageQueue(ep.QueuePath, QueueAccessMode.Receive))
            {
                Message msg = mq.Receive(TimeSpan.FromSeconds(3));
                msg.ShouldNotBeNull();

                object message = new BinaryFormatter().Deserialize(msg.BodyStream);

                message.ShouldNotBeNull();

                message.ShouldBeSameType<T>();
            }
        }

        public static void VerifyMessageInTransactionalQueue<T>(this MsmqEndpoint ep)
        {
            using (TransactionScope trx = new TransactionScope())
            {
                ep.VerifyMessageInQueue<T>();
                trx.Complete();
            }
        }

        public static void VerifyMessageNotInTransactionalQueue(this MsmqEndpoint ep)
        {
            using (TransactionScope trx = new TransactionScope())
            {
                ep.VerifyMessageNotInQueue();
                trx.Complete();
            }
        }
        public static void VerifyMessageNotInQueue(this MsmqEndpoint ep)
        {
            using (MessageQueue mq = new MessageQueue(ep.QueuePath, QueueAccessMode.Receive))
            {
                try
                {
                    Message msg = mq.Receive(TimeSpan.FromSeconds(0.1));
                    msg.ShouldNotBeNull();
                }
                catch (MessageQueueException ex)
                {
                    ex.Message
                        .ShouldEqual("Timeout for the requested operation has expired.");
                }
            }
        }
    }
}