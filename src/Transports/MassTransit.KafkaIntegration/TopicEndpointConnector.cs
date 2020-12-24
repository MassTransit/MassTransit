namespace MassTransit.KafkaIntegration
{
    using System;
    using Confluent.Kafka;
    using MassTransit.Registration;
    using Transports;


    public class TopicEndpointConnector :
        ITopicEndpointConnector
    {
        readonly IBusInstance _busInstance;
        readonly IConsumerSpecificationCreator _consumerSpecificationCreator;
        readonly IReceiveEndpointCollection _endpointCollection;

        public TopicEndpointConnector(IReceiveEndpointCollection endpointCollection, IBusInstance busInstance,
            IConsumerSpecificationCreator consumerSpecificationCreator)
        {
            _endpointCollection = endpointCollection;
            _busInstance = busInstance;
            _consumerSpecificationCreator = consumerSpecificationCreator;
        }

        public HostReceiveEndpointHandle ConnectTopicEndpoint<TKey, TValue>(string topicName, string groupId,
            Action<IKafkaTopicReceiveEndpointConfigurator<TKey, TValue>> configure)
            where TValue : class
        {
            var specification = _consumerSpecificationCreator.CreateSpecification(topicName, groupId, configure);
            _endpointCollection.Add(specification.EndpointName, specification.CreateReceiveEndpoint(_busInstance));
            return _endpointCollection.Start(specification.EndpointName);
        }

        public HostReceiveEndpointHandle ConnectTopicEndpoint<TKey, TValue>(string topicName, ConsumerConfig consumerConfig,
            Action<IKafkaTopicReceiveEndpointConfigurator<TKey, TValue>> configure)
            where TValue : class
        {
            var specification = _consumerSpecificationCreator.CreateSpecification(topicName, consumerConfig, configure);
            _endpointCollection.Add(specification.EndpointName, specification.CreateReceiveEndpoint(_busInstance));
            return _endpointCollection.Start(specification.EndpointName);
        }
    }
}
