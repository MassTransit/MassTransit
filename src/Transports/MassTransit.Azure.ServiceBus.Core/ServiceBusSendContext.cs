namespace MassTransit
{
    using System;


    public interface ServiceBusSendContext :
        SendContext,
        PartitionKeySendContext
    {
        /// <summary>
        /// Set the time at which the message should be enqueued, which is essentially scheduling the message for future delivery to the queue.
        /// </summary>
        DateTime? ScheduledEnqueueTimeUtc { set; }

        /// <summary>
        /// Set the sessionId of the message
        /// </summary>
        string SessionId { set; }

        /// <summary>
        /// Set the replyToSessionId of the message
        /// </summary>
        string ReplyToSessionId { set; }

        /// <summary>
        /// Sets the ReplyTo address of the message
        /// </summary>
        string ReplyTo { set; }

        /// <summary>
        /// Set the application specific label of the message
        /// </summary>
        string Label { set; }
    }


    public interface ServiceBusSendContext<out T> :
        SendContext<T>,
        ServiceBusSendContext
        where T : class
    {
    }
}
