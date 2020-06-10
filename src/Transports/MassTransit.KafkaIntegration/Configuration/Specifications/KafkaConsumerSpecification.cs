namespace MassTransit.KafkaIntegration.Specifications
{
    using System;
    using System.Collections.Generic;
    using Configurators;
    using Confluent.Kafka;
    using GreenPipes;
    using MassTransit.Configurators;
    using MassTransit.Registration;
    using Pipeline.Observables;
    using Serializers;
    using Transport;


    public class KafkaConsumerSpecification<TKey, TValue> :
        IKafkaConsumerSpecification
        where TValue : class
    {
        readonly Action<IKafkaTopicReceiveEndpointConfigurator<TKey, TValue>> _configure;
        readonly ConsumerConfig _consumerConfig;
        readonly ReceiveEndpointObservable _endpointObservers;
        readonly IHeadersDeserializer _headersDeserializer;

        public KafkaConsumerSpecification(ConsumerConfig consumerConfig, string topicName, IHeadersDeserializer headersDeserializer,
            Action<IKafkaTopicReceiveEndpointConfigurator<TKey, TValue>> configure)
        {
            _consumerConfig = consumerConfig;
            Name = topicName;
            _endpointObservers = new ReceiveEndpointObservable();
            _headersDeserializer = headersDeserializer;
            _configure = configure;
        }

        public string Name { get; }

        public IKafkaReceiveEndpoint CreateReceiveEndpoint(IBusInstance busInstance)
        {
            var endpointConfiguration = busInstance.HostConfiguration.CreateReceiveEndpointConfiguration($"{KafkaTopicAddress.PathPrefix}/{Name}");
            endpointConfiguration.ConnectReceiveEndpointObserver(_endpointObservers);

            var configurator =
                new KafkaTopicReceiveEndpointConfiguration<TKey, TValue>(_consumerConfig, Name, busInstance, endpointConfiguration, _headersDeserializer);
            _configure?.Invoke(configurator);

            var result = BusConfigurationResult.CompileResults(configurator.Validate());

            try
            {
                return configurator.Build();
            }
            catch (Exception ex)
            {
                throw new ConfigurationException(result, $"An exception occurred creating the {nameof(IKafkaReceiveEndpoint)}", ex);
            }
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrEmpty(Name))
                yield return this.Failure("Topic", "should not be empty");

            if (string.IsNullOrEmpty(_consumerConfig.GroupId))
                yield return this.Failure("GroupId", "should not be empty");

            if (string.IsNullOrEmpty(_consumerConfig.BootstrapServers))
                yield return this.Failure("BootstrapServers", "should not be empty. Please use cfg.Host() to configure it");
        }

        public ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _endpointObservers.Connect(observer);
        }
    }
}
