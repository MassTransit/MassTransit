namespace MassTransit.AmazonSqsTransport.Topology
{
    using System.Collections.Generic;


    /// <summary>
    /// The queue details used to declare the queue to AmazonSQS
    /// </summary>
    public interface Queue
    {
        /// <summary>
        /// The queue name
        /// </summary>
        string EntityName { get; }

        /// <summary>
        /// True if the queue should be durable, and survive a broker restart
        /// </summary>
        bool Durable { get; }

        /// <summary>
        /// True if the queue should be deleted when the connection is closed
        /// </summary>
        bool AutoDelete { get; }

        /// <summary>
        /// Additional <see href="https://docs.aws.amazon.com/AWSSimpleQueueService/latest/APIReference/API_SetQueueAttributes.html">attributes</see> for the queue.
        /// </summary>
        IDictionary<string, object> QueueAttributes { get; }

        /// <summary>
        /// Additional <see href="https://docs.aws.amazon.com/sns/latest/api/API_SetSubscriptionAttributes.html">attributes</see> for the queue's subscription.
        /// </summary>
        IDictionary<string, object> QueueSubscriptionAttributes { get; }

        /// <summary>
        /// Collection of tags to assign to queue when created.
        /// </summary>
        IDictionary<string, string> QueueTags { get; }
    }
}
