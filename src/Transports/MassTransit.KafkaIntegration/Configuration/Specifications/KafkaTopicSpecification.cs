namespace MassTransit.KafkaIntegration.Specifications
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using Configurators;
    using Confluent.Kafka;
    using GreenPipes;
    using MassTransit.Configurators;
    using MassTransit.Registration;
    using Pipeline.Observables;
    using Serializers;
    using Transport;


    public class KafkaTopicSpecification<TKey, TValue> :
        IKafkaTopicSpecification
        where TValue : class
    {
        readonly Action<IKafkaTopicConfigurator<TKey, TValue>> _configure;
        readonly ConsumerConfig _consumerConfig;
        readonly ReceiveEndpointObservable _endpointObservers;
        readonly IHeadersDeserializer _headersDeserializer;

        public KafkaTopicSpecification(ConsumerConfig consumerConfig, ITopicNameFormatter topicNameFormatter, IHeadersDeserializer headersDeserializer,
            Action<IKafkaTopicConfigurator<TKey, TValue>> configure)
        {
            _consumerConfig = consumerConfig;
            Name = topicNameFormatter.GetTopicName<TKey, TValue>();
            _endpointObservers = new ReceiveEndpointObservable();
            _headersDeserializer = headersDeserializer;
            _configure = configure;
        }

        public string Name { get; }

        public IKafkaReceiveEndpoint CreateEndpoint(IBusInstance busInstance)
        {
            var endpointConfiguration = busInstance.HostConfiguration.CreateReceiveEndpointConfiguration($"kafka/{Name}");
            endpointConfiguration.ConnectReceiveEndpointObserver(_endpointObservers);

            var configurator = new KafkaTopicConfigurator<TKey, TValue>(_consumerConfig, Name, busInstance, endpointConfiguration, _headersDeserializer);
            _configure?.Invoke(configurator);

            var result = BusConfigurationResult.CompileResults(configurator.Validate());

            try
            {
                return configurator.Build();
            }
            catch (Exception ex)
            {
                throw new ConfigurationException(result, "An exception occurred creating the EventDataReceiver", ex);
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
