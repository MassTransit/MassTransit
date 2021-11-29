namespace MassTransit
{
    using System.Collections.Generic;


    public interface IAmazonSqsPublishTopology :
        IPublishTopology
    {
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

        new IAmazonSqsMessagePublishTopology<T> GetMessageTopology<T>()
            where T : class;
    }
}
