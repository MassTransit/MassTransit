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
        readonly IRiderRegistrationContext _context;
        readonly IReceiveEndpointCollection _endpointCollection;

        public TopicEndpointConnector(IRiderRegistrationContext context, IReceiveEndpointCollection endpointCollection, IBusInstance busInstance,
            IConsumerSpecificationCreator consumerSpecificationCreator)
        {
            _context = context;
            _endpointCollection = endpointCollection;
            _busInstance = busInstance;
            _consumerSpecificationCreator = consumerSpecificationCreator;
        }

        public HostReceiveEndpointHandle ConnectTopicEndpoint<TKey, TValue>(string topicName, string groupId,
            Action<IRiderRegistrationContext, IKafkaTopicReceiveEndpointConfigurator<TKey, TValue>> configure)
            where TValue : class
        {
            var configurator = new Configurator<TKey, TValue>(_context, configure);
            var specification = _consumerSpecificationCreator.CreateSpecification<TKey, TValue>(topicName, groupId, configurator.Configure);
            _endpointCollection.Add(specification.EndpointName, specification.CreateReceiveEndpoint(_busInstance));
            return _endpointCollection.Start(specification.EndpointName);
        }

        public HostReceiveEndpointHandle ConnectTopicEndpoint<TKey, TValue>(string topicName, ConsumerConfig consumerConfig,
            Action<IRiderRegistrationContext, IKafkaTopicReceiveEndpointConfigurator<TKey, TValue>> configure)
            where TValue : class
        {
            var configurator = new Configurator<TKey, TValue>(_context, configure);
            var specification = _consumerSpecificationCreator.CreateSpecification<TKey, TValue>(topicName, consumerConfig, configurator.Configure);
            _endpointCollection.Add(specification.EndpointName, specification.CreateReceiveEndpoint(_busInstance));
            return _endpointCollection.Start(specification.EndpointName);
        }


        class Configurator<TKey, TValue>
            where TValue : class
        {
            readonly Action<IRiderRegistrationContext, IKafkaTopicReceiveEndpointConfigurator<TKey, TValue>> _configure;
            readonly IRiderRegistrationContext _context;

            public Configurator(IRiderRegistrationContext context,
                Action<IRiderRegistrationContext, IKafkaTopicReceiveEndpointConfigurator<TKey, TValue>> configure)
            {
                _context = context;
                _configure = configure;
            }

            public void Configure(IKafkaTopicReceiveEndpointConfigurator<TKey, TValue> configurator)
            {
                _configure?.Invoke(_context, configurator);
            }
        }
    }
}
