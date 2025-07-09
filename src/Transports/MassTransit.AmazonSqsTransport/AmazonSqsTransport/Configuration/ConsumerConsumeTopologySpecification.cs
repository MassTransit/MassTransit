namespace MassTransit.AmazonSqsTransport.Configuration;

using System.Collections.Generic;
using Internals;
using Topology;


/// <summary>
/// Used to by a TopicSubscription destination to the receive endpoint, via an additional message consumer
/// </summary>
public class ConsumerConsumeTopologySpecification :
    AmazonSqsTopicSubscriptionConfigurator,
    IAmazonSqsConsumeTopologySpecification
{
    readonly IAmazonSqsPublishTopology _publishTopology;

    public ConsumerConsumeTopologySpecification(IAmazonSqsPublishTopology publishTopology, string topicName, bool durable = true, bool autoDelete = false)
        : base(topicName, durable, autoDelete)
    {
        _publishTopology = publishTopology;
    }

    public ConsumerConsumeTopologySpecification(IAmazonSqsPublishTopology publishTopology, Topic topic)
        : base(topic)
    {
        _publishTopology = publishTopology;
    }

    public IEnumerable<ValidationResult> Validate()
    {
        return [];
    }

    public void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
    {
        var topicHandle = builder.CreateTopic(EntityName, Durable, AutoDelete,
            _publishTopology.TopicAttributes.MergeLeft(TopicAttributes),
            _publishTopology.TopicSubscriptionAttributes.MergeLeft(TopicSubscriptionAttributes),
            _publishTopology.TopicTags.MergeLeft(Tags));


        if (builder.Queue != null)
            builder.CreateQueueSubscription(topicHandle, builder.Queue);
    }
}
