namespace MassTransit.AzureServiceBusTransport.Topology
{
    public static class EntityNameFormatter
    {
        const string PathDelimiter = @"/";
        const string Subscriptions = "Subscriptions";
        const string SubQueuePrefix = "$";
        const string DeadLetterQueueSuffix = "DeadLetterQueue";
        const string ErrorQueueSuffix = "Error";
        const string DeadLetterQueueName = SubQueuePrefix + DeadLetterQueueSuffix;
        const string ErrorQueueName = SubQueuePrefix + ErrorQueueSuffix;

        /// <summary>
        /// Formats the dead letter path for either a queue, or a subscription.
        /// </summary>
        /// <param name="entityPath">The name of the queue, or path of the subscription.</param>
        /// <returns>The path as a string of the dead letter entity.</returns>
        public static string FormatDeadLetterPath(string entityPath)
        {
            return FormatSubQueuePath(entityPath, DeadLetterQueueName);
        }

        /// <summary>
        /// Formats the dead letter path for either a queue, or a subscription.
        /// </summary>
        /// <param name="entityPath">The name of the queue, or path of the subscription.</param>
        /// <returns>The path as a string of the dead letter entity.</returns>
        public static string FormatErrorPath(string entityPath)
        {
            return FormatSubQueuePath(entityPath, ErrorQueueName);
        }

        /// <summary>
        /// Formats the subqueue path for either a queue, or a subscription.
        /// </summary>
        /// <param name="entityPath">The name of the queue, or path of the subscription.</param>
        /// <param name="subQueueName">The name of the subQueue</param>
        /// <returns>The path as a string of the subqueue entity.</returns>
        public static string FormatSubQueuePath(string entityPath, string subQueueName)
        {
            return string.Concat(entityPath, PathDelimiter, subQueueName);
        }

        /// <summary>
        /// Formats the subscription path, based on the topic path and subscription name.
        /// </summary>
        /// <param name="topicPath">The name of the topic, including slashes.</param>
        /// <param name="subscriptionName">The subscription name</param>
        public static string FormatSubscriptionPath(string topicPath, string subscriptionName)
        {
            return string.Concat(topicPath, PathDelimiter, Subscriptions, PathDelimiter, subscriptionName);
        }
    }
}
