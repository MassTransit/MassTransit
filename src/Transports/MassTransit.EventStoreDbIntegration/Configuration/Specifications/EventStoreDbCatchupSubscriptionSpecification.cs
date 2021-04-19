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
    public class EventStoreDbCatchupSubscriptionSpecification :
        IEventStoreDbSubscriptionSpecification
    {
        readonly Action<IEventStoreDbCatchupSubscriptionConfigurator> _configure;      
        readonly ReceiveEndpointObservable _endpointObservers;      
        readonly IHeadersDeserializer _headersDeserializer;
        readonly IEventStoreDbHostConfiguration _hostConfiguration;
        readonly IHostSettings _hostSettings;
        readonly StreamCategory _streamCategory;
        readonly string _subscriptionName;

        public EventStoreDbCatchupSubscriptionSpecification(IEventStoreDbHostConfiguration hostConfiguration, StreamCategory streamCategory,
            string subscriptionName, IHostSettings hostSettings, IHeadersDeserializer headersDeserializer,
            Action<IEventStoreDbCatchupSubscriptionConfigurator> configure)
        {
            _hostConfiguration = hostConfiguration;
            _streamCategory = streamCategory;
            _subscriptionName = subscriptionName;
            _hostSettings = hostSettings;
            _headersDeserializer = headersDeserializer;
            _configure = configure;
            EndpointName = $"{EventStoreDbEndpointAddress.PathPrefix}/{_streamCategory}/{_subscriptionName}";

            _endpointObservers = new ReceiveEndpointObservable();
        }

        public string EndpointName { get; }

        public ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _endpointObservers.Connect(observer);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrWhiteSpace(_streamCategory))
                yield return this.Failure("StreamCategory", "should not be empty");

            if (string.IsNullOrWhiteSpace(_subscriptionName))
                yield return this.Failure("SubscriptionName", "should not be empty");
        }

        public ReceiveEndpoint CreateReceiveEndpoint(IBusInstance busInstance)
        {
            var endpointConfiguration = busInstance.HostConfiguration.CreateReceiveEndpointConfiguration(EndpointName);
            endpointConfiguration.ConnectReceiveEndpointObserver(_endpointObservers);

            var configurator = new EventStoreDbCatchupSubscriptionConfigurator(_hostConfiguration, _streamCategory, _subscriptionName, busInstance,
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
