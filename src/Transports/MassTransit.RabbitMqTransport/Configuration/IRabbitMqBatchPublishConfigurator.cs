namespace MassTransit
{
    using System;


    public interface IRabbitMqBatchPublishConfigurator
    {
        /// <summary>
        /// If true, messages are queued up to send in batches to reduce broker round-trip calls
        /// </summary>
        bool Enabled { set; }

        /// <summary>
        /// The maximum number of messages to include in a batch
        /// </summary>
        int MessageLimit { set; }

        /// <summary>
        /// A rough size limit for a batch of messages
        /// </summary>
        int SizeLimit { set; }

        /// <summary>
        /// The time to wait for more messages before sending a batch. Should be small, like &lt; 10 milliseconds
        /// </summary>
        TimeSpan Timeout { set; }
    }
}
