namespace MassTransit.KafkaIntegration.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Confluent.Kafka;
    using Observables;
    using Serializers;
    using Transports;


    public class KafkaConsumerSpecification<TKey, TValue> :
        IKafkaConsumerSpecification
        where TValue : class
    {
        readonly Action<IKafkaTopicReceiveEndpointConfigurator<TKey, TValue>> _configure;
        readonly ConsumerConfig _consumerConfig;
        readonly ReceiveEndpointObservable _endpointObservers;
        readonly IHeadersDeserializer _headersDeserializer;
        readonly IKafkaHostConfiguration _hostConfiguration;
        readonly IKafkaSerializerFactory _kafkaSerializerFactory;
        readonly Action<IClient, string> _oAuthBearerTokenRefreshHandler;
        readonly string _topicName;

        public KafkaConsumerSpecification(IKafkaHostConfiguration hostConfiguration, ConsumerConfig consumerConfig, string topicName,
            IHeadersDeserializer headersDeserializer, IKafkaSerializerFactory kafkaSerializerFactory,
            Action<IKafkaTopicReceiveEndpointConfigurator<TKey, TValue>> configure,
            Action<IClient, string> oAuthBearerTokenRefreshHandler)
        {
            _hostConfiguration = hostConfiguration;
            _consumerConfig = consumerConfig;
            _topicName = topicName;
            _endpointObservers = new ReceiveEndpointObservable();
            _headersDeserializer = headersDeserializer;
            _kafkaSerializerFactory = kafkaSerializerFactory;
            _configure = configure;
            _oAuthBearerTokenRefreshHandler = oAuthBearerTokenRefreshHandler;
            EndpointName = $"{KafkaTopicAddress.PathPrefix}/{_topicName}";
            if (!string.IsNullOrWhiteSpace(_consumerConfig.GroupId))
                EndpointName = $"{EndpointName}/{_consumerConfig.GroupId}";
        }

        public string EndpointName { get; }

        public ReceiveEndpoint CreateReceiveEndpoint(IBusInstance busInstance)
        {
            var endpointConfiguration = busInstance.HostConfiguration.CreateReceiveEndpointConfiguration(EndpointName);

            var configurator = new KafkaTopicReceiveEndpointConfiguration<TKey, TValue>(_hostConfiguration, _consumerConfig, _topicName, busInstance,
                endpointConfiguration, _oAuthBearerTokenRefreshHandler);
            configurator.ConnectReceiveEndpointObserver(_endpointObservers);
            configurator.SetHeadersDeserializer(_headersDeserializer);
            configurator.SetKeyDeserializer(_kafkaSerializerFactory.GetDeserializer<TKey>());
            configurator.SetValueDeserializer(_kafkaSerializerFactory.GetDeserializer<TValue>());
            _configure?.Invoke(configurator);

            IReadOnlyList<ValidationResult> result = Validate().Concat(configurator.Validate())
                .ThrowIfContainsFailure($"{TypeCache.GetShortName(GetType())} configuration is invalid:");

            try
            {
                return configurator.Build();
            }
            catch (Exception ex)
            {
                throw new ConfigurationException(result, "An exception occurred creating Kafka receive endpoint", ex);
            }
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrEmpty(_topicName))
                yield return this.Failure("Topic", "should not be empty");

            if (string.IsNullOrEmpty(_consumerConfig.GroupId))
                yield return this.Failure("GroupId", "should not be empty");
        }

        public ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _endpointObservers.Connect(observer);
        }
    }
}
