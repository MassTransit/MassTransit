using System;
using System.Collections.Generic;
using GreenPipes;
using MassTransit.Configurators;
using MassTransit.EventStoreDbIntegration.Configurators;
using MassTransit.Pipeline.Observables;
using MassTransit.Registration;
using MassTransit.Transports;

namespace MassTransit.EventStoreDbIntegration.Specifications
{
    public class EventStoreDbReceiveEndpointSpecification :
        IEventStoreDbReceiveEndpointSpecification
    {
        readonly Action<IEventStoreDbReceiveEndpointConfigurator> _configure;
        readonly string _subscriptionName;
        readonly ReceiveEndpointObservable _endpointObservers;
        readonly StreamCategory _streamCategory;
        readonly IEventStoreDbHostConfiguration _hostConfiguration;
        readonly IHostSettings _hostSettings;

        public EventStoreDbReceiveEndpointSpecification(IEventStoreDbHostConfiguration hostConfiguration, StreamCategory streamCategory, string subscriptionName,
            IHostSettings hostSettings,
            Action<IEventStoreDbReceiveEndpointConfigurator> configure)
        {
            _hostConfiguration = hostConfiguration;
            _streamCategory = streamCategory;
            _subscriptionName = subscriptionName;
            _hostSettings = hostSettings;
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

            if (!_hostSettings.UseExistingClient && string.IsNullOrWhiteSpace(_hostSettings.ConnectionString)
                && string.IsNullOrWhiteSpace(_hostSettings.ConnectionName))
                yield return this.Failure("HostSettings", "is invalid");
        }

        public ReceiveEndpoint CreateReceiveEndpoint(IBusInstance busInstance)
        {
            var endpointConfiguration = busInstance.HostConfiguration.CreateReceiveEndpointConfiguration(EndpointName);
            endpointConfiguration.ConnectReceiveEndpointObserver(_endpointObservers);

            var configurator = new EventStoreDbReceiveEndpointConfigurator(_hostConfiguration, _streamCategory, _subscriptionName, busInstance, endpointConfiguration);
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
