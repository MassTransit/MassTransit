namespace MassTransit.Transports.Msmq.Tests
{
    using System;
    using System.Messaging;
    using System.Runtime.Serialization.Formatters.Binary;
    using MassTransit.Tests;

    public static class MsmqEndpoint_Extensions
    {
        public static void Purge(this MsmqEndpoint ep)
        {
            try
            {
                MessageQueue queue = new MessageQueue(ep.QueuePath, QueueAccessMode.ReceiveAndAdmin);
                queue.Purge();
            }
            catch (Exception)
            {
                //ignore?
            }
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
    }
}