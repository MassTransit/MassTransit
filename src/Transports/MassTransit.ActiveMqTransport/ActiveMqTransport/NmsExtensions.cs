namespace MassTransit.ActiveMqTransport
{
    using Apache.NMS;
    using Apache.NMS.ActiveMQ;
    using Apache.NMS.ActiveMQ.Commands;
    using Apache.NMS.AMQP;
    using Apache.NMS.AMQP.Message;


    static class NmsExtensions
    {
        public static void Stop(this IMessageConsumer consumer)
        {
            if (consumer is MessageConsumer activeMqConsumer)
                activeMqConsumer.Stop();
            else if (consumer is NmsMessageConsumer nmsConsumer)
                nmsConsumer.Stop();
        }

        public static void Start(this IMessageConsumer consumer)
        {
            if (consumer is MessageConsumer activeMqConsumer)
                activeMqConsumer.Start();
            else if (consumer is NmsMessageConsumer nmsConsumer)
                nmsConsumer.Start();
        }

        public static string GetGroupId(this IMessage message)
        {
            if (message is NmsMessage nmsMessage)
                return nmsMessage.NMSXGroupId;
            if (message is ActiveMQMessage activeMqMessage)
                return activeMqMessage.GroupID;
            return null;
        }

        public static int GetGroupSequence(this IMessage message)
        {
            if (message is NmsMessage nmsMessage)
                return nmsMessage.NMSXGroupSeq;
            if (message is ActiveMQMessage activeMqMessage)
                return activeMqMessage.GroupSequence;
            return default;
        }

        public static void SetGroupId(this IMessage message, string groupId)
        {
            if (message is NmsMessage nmsMessage)
                nmsMessage.NMSXGroupId = groupId;
            else if (message is ActiveMQMessage activeMqMessage)
                activeMqMessage.GroupID = groupId;
        }

        public static void SetGroupSequence(this IMessage message, int groupSequence)
        {
            if (message is NmsMessage nmsMessage)
                nmsMessage.NMSXGroupSeq = groupSequence;
            else if (message is ActiveMQMessage activeMqMessage)
                activeMqMessage.GroupSequence = groupSequence;
        }
    }
}
