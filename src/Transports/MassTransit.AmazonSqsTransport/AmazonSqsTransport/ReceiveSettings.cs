namespace MassTransit.AmazonSqsTransport
{
    using System;
    using System.Collections.Generic;


    /// <summary>
    /// Specify the receive settings for a receive transport
    /// </summary>
    public interface ReceiveSettings :
        EntitySettings
    {
        /// <summary>
        /// The number of unacknowledged messages to allow to be processed concurrently
        /// </summary>
        int PrefetchCount { get; }

        int ConcurrentMessageLimit { get; }

        int WaitTimeSeconds { get; }

        /// <summary>
        /// If True, and a queue name is specified, if the queue exists and has messages, they are purged at startup
        /// If the connection is reset, messages are not purged until the service is reset
        /// </summary>
        bool PurgeOnStartup { get; }

        /// <summary>
        /// Additional <see href="https://docs.aws.amazon.com/AWSSimpleQueueService/latest/APIReference/API_SetQueueAttributes.html">attributes</see> for the queue.
        /// </summary>
        IDictionary<string, object> QueueAttributes { get; }

        /// <summary>
        /// Additional <see href="https://docs.aws.amazon.com/sns/latest/api/API_SetSubscriptionAttributes.html">attributes</see> for the queue's subscription.
        /// </summary>
        IDictionary<string, object> QueueSubscriptionAttributes { get; }

        /// <summary>
        /// Get the input address for the transport on the specified host
        /// </summary>
        Uri GetInputAddress(Uri hostAddress);

        /// <summary>
        /// If the queue is ordered, enables grouping by MessageGroupId and process messages in ordered way by SequenceNumber
        /// </summary>
        bool IsOrdered { get; }

        int VisibilityTimeout { get; set; }

        string QueueUrl { get; set; }
    }
}
