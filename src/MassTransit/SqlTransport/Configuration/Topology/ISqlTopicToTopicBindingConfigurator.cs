#nullable enable
namespace MassTransit
{
    using System;


    public interface ISqlTopicToTopicBindingConfigurator :
        ISqlTopicSubscriptionConfigurator
    {
        /// <summary>
        /// Creates a subscription between two topics
        /// </summary>
        /// <param name="topicName">Topic name of the new exchange</param>
        /// <param name="configure">Configuration for new exchange and how to bind to it</param>
        void Subscribe(string topicName, Action<ISqlTopicToTopicBindingConfigurator>? configure = null);
    }
}
