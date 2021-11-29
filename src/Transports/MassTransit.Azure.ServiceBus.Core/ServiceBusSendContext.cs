namespace MassTransit
{
    using System;


    public interface ServiceBusSendContext :
        SendContext
    {
        /// <summary>
        /// Set the time at which the message should be enqueued, which is essentially scheduling the message for future delivery to the queue.
        /// </summary>
        DateTime? ScheduledEnqueueTimeUtc { set; }

        /// <summary>
        /// Set the partition key for the message, which is used to split load across nodes in Azure
        /// </summary>
        string PartitionKey { set; }

        /// <summary>
        /// Set the sessionId of the message
        /// </summary>
        string SessionId { set; }

        /// <summary>
        /// Set the replyToSessionId of the message
        /// </summary>
        string ReplyToSessionId { set; }
    }


    public interface ServiceBusSendContext<out T> :
        SendContext<T>,
        ServiceBusSendContext
        where T : class
    {
    }
}
