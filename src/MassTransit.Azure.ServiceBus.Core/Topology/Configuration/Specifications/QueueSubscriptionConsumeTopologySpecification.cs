namespace MassTransit.Azure.ServiceBus.Core.Topology.Specifications
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using Builders;
    using GreenPipes;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Management;
    using Util;


    /// <summary>
    /// Used to bind an exchange to the consuming queue's exchange
    /// </summary>
    public class SubscriptionConsumeTopologySpecification :
        IServiceBusConsumeTopologySpecification
    {
        readonly Filter _filter;
        readonly RuleDescription _rule;
        readonly SubscriptionDescription _subscriptionDescription;
        readonly TopicDescription _topicDescription;

        public SubscriptionConsumeTopologySpecification(TopicDescription topicDescription, SubscriptionDescription subscriptionDescription,
            RuleDescription rule, Filter filter)
        {
            _topicDescription = topicDescription;
            _subscriptionDescription = subscriptionDescription;
            _rule = rule;
            _filter = filter;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }

        public void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
        {
            var topic = builder.CreateTopic(_topicDescription);

            var subscriptionDescription = _subscriptionDescription;

            subscriptionDescription.ForwardTo = builder.Queue.Queue.QueueDescription.Path;
            subscriptionDescription.SubscriptionName =
                GetSubscriptionName(subscriptionDescription.SubscriptionName, builder.Queue.Queue.QueueDescription.Path.Split('/').Last());

            builder.CreateQueueSubscription(topic, builder.Queue, subscriptionDescription, _rule, _filter);
        }

        static string GetSubscriptionName(string subscriptionName, string queuePath)
        {
            var subscriptionPath = subscriptionName.Replace("{queuePath}", queuePath);

            string name;
            if (subscriptionPath.Length > 50)
            {
                string hashed;
                using (var hasher = new SHA1Managed())
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(subscriptionPath);
                    byte[] hash = hasher.ComputeHash(buffer);
                    hashed = FormatUtil.Formatter.Format(hash).Substring(0, 6);
                }

                name = $"{subscriptionPath.Substring(0, 43)}-{hashed}";
            }
            else
                name = subscriptionPath;

            return name;
        }
    }
}
