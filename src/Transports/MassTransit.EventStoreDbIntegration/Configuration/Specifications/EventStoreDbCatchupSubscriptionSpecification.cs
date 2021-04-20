using System;
using System.Collections.Generic;
using GreenPipes;
using MassTransit.Configurators;
using MassTransit.EventStoreDbIntegration.Configurators;
using MassTransit.EventStoreDbIntegration.Serializers;
using MassTransit.Pipeline.Observables;
using MassTransit.Registration;
using MassTransit.Transports;

namespace MassTransit.EventStoreDbIntegration.Specifications
{
    public sealed class EventStoreDbCatchupSubscriptionSpecification :
        IEventStoreDbSubscriptionSpecification
    {
        readonly Action<IEventStoreDbCatchupSubscriptionConfigurator> _configure;      
        readonly ReceiveEndpointObservable _endpointObservers;      
        readonly IHeadersDeserializer _headersDeserializer;
        readonly IEventStoreDbHostConfiguration _hostConfiguration;
        readonly StreamName _streamName;
        readonly string _subscriptionName;

        public EventStoreDbCatchupSubscriptionSpecification(IEventStoreDbHostConfiguration hostConfiguration, StreamName streamName,
            string subscriptionName, IHeadersDeserializer headersDeserializer,
            Action<IEventStoreDbCatchupSubscriptionConfigurator> configure)
        {
            _hostConfiguration = hostConfiguration;
            _streamName = streamName;
            _subscriptionName = subscriptionName;
            _headersDeserializer = headersDeserializer;
            _configure = configure;
            EndpointName = $"{EventStoreDbEndpointAddress.PathPrefix}/{_streamName}/{_subscriptionName}";

            _endpointObservers = new ReceiveEndpointObservable();
        }

        public string EndpointName { get; }

        public ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _endpointObservers.Connect(observer);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_streamName == null)
                yield return this.Failure("StreamName", "should not be empty");

            if (string.IsNullOrWhiteSpace(_subscriptionName))
                yield return this.Failure("SubscriptionName", "should not be empty");
        }

        public ReceiveEndpoint CreateReceiveEndpoint(IBusInstance busInstance)
        {
            var endpointConfiguration = busInstance.HostConfiguration.CreateReceiveEndpointConfiguration(EndpointName);
            endpointConfiguration.ConnectReceiveEndpointObserver(_endpointObservers);

            var configurator = new EventStoreDbCatchupSubscriptionConfigurator(_hostConfiguration, _streamName, _subscriptionName, busInstance,
                endpointConfiguration, _headersDeserializer);
            _configure?.Invoke(configurator);

            var result = BusConfigurationResult.CompileResults(configurator.Validate());

            try
            {
                return configurator.Build();
            }
            catch (Exception ex)
            {
                throw new ConfigurationException(result, "An exception occurred creating the EventStoreDB receive endpoint", ex);
            }
        }
    }
}
