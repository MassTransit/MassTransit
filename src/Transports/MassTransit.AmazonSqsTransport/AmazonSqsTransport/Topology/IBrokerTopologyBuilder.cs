namespace MassTransit.AmazonSqsTransport.Topology
{
    using System.Collections.Generic;


    public interface IBrokerTopologyBuilder
    {
        /// <summary>
        /// Declares an exchange
        /// </summary>
        /// <param name="name">The topic name</param>
        /// <param name="durable">A durable topic survives a broker restart</param>
        /// <param name="autoDelete">Automatically delete if the broker connection is closed</param>
        /// <param name="topicAttributes"></param>
        /// <param name="topicSubscriptionAttributes"></param>
        /// <param name="tags"></param>
        /// <returns>An entity handle used to reference the topic in subsequent calls</returns>
        TopicHandle CreateTopic(string name, bool durable, bool autoDelete, IDictionary<string, object> topicAttributes = null,
            IDictionary<string, object> topicSubscriptionAttributes = null, IDictionary<string, string> tags = null);

        /// <summary>
        /// Declares a queue
        /// </summary>
        /// <param name="name"></param>
        /// <param name="durable"></param>
        /// <param name="autoDelete"></param>
        /// <param name="queueAttributes"></param>
        /// <param name="queueSubscriptionAttributes"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        QueueHandle CreateQueue(string name, bool durable, bool autoDelete, IDictionary<string, object> queueAttributes = null,
            IDictionary<string, object> queueSubscriptionAttributes = null, IDictionary<string, string> tags = null);

        /// <summary>
        /// Create a subscription on a topic to a queue
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="queue"></param>
        /// <returns></returns>
        QueueSubscriptionHandle CreateQueueSubscription(TopicHandle topic, QueueHandle queue);

        /// <summary>
        /// Create a subscription on a topic to another topic
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        TopicSubscriptionHandle CreateTopicSubscription(TopicHandle source, TopicHandle destination);
    }
}
