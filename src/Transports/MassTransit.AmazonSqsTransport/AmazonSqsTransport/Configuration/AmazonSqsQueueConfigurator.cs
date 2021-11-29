namespace MassTransit.AmazonSqsTransport.Configuration
{
    using System.Collections.Generic;
    using Amazon.SQS;
    using Topology;


    public class AmazonSqsQueueConfigurator :
        EntityConfigurator,
        IAmazonSqsQueueConfigurator,
        Queue
    {
        protected AmazonSqsQueueConfigurator(string queueName, bool durable = true, bool autoDelete = false, IDictionary<string, object> queueAttributes = null,
            IDictionary<string, object> queueSubscriptionAttributes = null, IDictionary<string, string> queueTags = null)
            : base(queueName, durable, autoDelete)
        {
            QueueAttributes = queueAttributes ?? new Dictionary<string, object>();
            QueueSubscriptionAttributes = queueSubscriptionAttributes ?? new Dictionary<string, object>();
            QueueTags = queueTags ?? new Dictionary<string, string>();

            if (AmazonSqsEndpointAddress.IsFifo(queueName))
                QueueAttributes[QueueAttributeName.FifoQueue] = "true";
        }

        public AmazonSqsQueueConfigurator(Queue source)
            : base(source.EntityName, source.Durable, source.AutoDelete)
        {
            QueueAttributes = source.QueueAttributes;
            QueueSubscriptionAttributes = source.QueueSubscriptionAttributes;
            QueueTags = source.QueueTags;
        }

        public IDictionary<string, string> Tags => QueueTags;

        protected override AmazonSqsEndpointAddress.AddressType AddressType => AmazonSqsEndpointAddress.AddressType.Queue;

        public IDictionary<string, object> QueueAttributes { get; set; }
        public IDictionary<string, object> QueueSubscriptionAttributes { get; set; }
        public IDictionary<string, string> QueueTags { get; set; }
    }
}
