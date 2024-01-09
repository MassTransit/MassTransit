#nullable enable
namespace MassTransit;

using System;
using System.ComponentModel;
using SqlTransport.Configuration;


public interface ISqlConsumeTopologyConfigurator :
    IConsumeTopologyConfigurator,
    ISqlConsumeTopology
{
    new ISqlMessageConsumeTopologyConfigurator<T> GetMessageTopology<T>()
        where T : class;

    [EditorBrowsable(EditorBrowsableState.Never)]
    void AddSpecification(ISqlConsumeTopologySpecification specification);

    /// <summary>
    /// Bind an exchange, using the configurator
    /// </summary>
    /// <param name="topicName"></param>
    /// <param name="configure"></param>
    void Subscribe(string topicName, Action<ISqlTopicSubscriptionConfigurator>? configure = null);
}
