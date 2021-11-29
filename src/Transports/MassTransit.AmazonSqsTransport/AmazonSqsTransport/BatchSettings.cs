namespace MassTransit.AmazonSqsTransport
{
    using System;


    public interface BatchSettings
    {
        /// <summary>
        /// If true, messages are queued up to send in batches to reduce broker round-trip calls
        /// </summary>
        bool Enabled { get; }

        /// <summary>
        /// The maximum number of messages to include in a batch
        /// </summary>
        int MessageLimit { get; }

        /// <summary>
        /// The max number of batches to send at one time
        /// </summary>
        int BatchLimit { get; }

        /// <summary>
        /// A rough size limit for a batch of messages
        /// </summary>
        int SizeLimit { get; }

        /// <summary>
        /// The time to wait for more messages before sending a batch. Should be small, like &lt; 10 milliseconds
        /// </summary>
        TimeSpan Timeout { get; }
    }
}
