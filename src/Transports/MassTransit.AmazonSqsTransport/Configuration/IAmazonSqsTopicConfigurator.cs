namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using AmazonSqsTransport;


    /// <summary>
    /// Configures an exchange for AmazonSQS
    /// </summary>
    public interface IAmazonSqsTopicConfigurator
    {
        /// <summary>
        /// Specify the queue should be durable (survives broker restart) or in-memory
        /// </summary>
        /// <value>True for a durable queue, False for an in-memory queue</value>
        bool Durable { set; }

        /// <summary>
        /// Specify that the queue (and the exchange of the same name) should be created as auto-delete
        /// </summary>
        bool AutoDelete { set; }

        /// <summary>
        /// Additional <see href="https://docs.aws.amazon.com/sns/latest/api/API_SetTopicAttributes.html">attributes</see> for the topic.
        /// </summary>
        IDictionary<string, object> TopicAttributes { get; }

        /// <summary>
        /// Additional <see href="https://docs.aws.amazon.com/sns/latest/api/API_SetSubscriptionAttributes.html">attributes</see> for the topic's subscription.
        /// </summary>
        IDictionary<string, object> TopicSubscriptionAttributes { get; }

        /// <summary>
        /// Collection of tags to assign to topic when created.
        /// </summary>
        IDictionary<string, string> TopicTags { get; }

        AmazonSqsEndpointAddress GetEndpointAddress(Uri hostAddress);
    }
}
