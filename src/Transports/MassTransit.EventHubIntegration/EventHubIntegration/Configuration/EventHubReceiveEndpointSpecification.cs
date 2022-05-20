namespace MassTransit.EventHubIntegration.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Observables;
    using Transports;


    public class EventHubReceiveEndpointSpecification :
        IEventHubReceiveEndpointSpecification
    {
        readonly Action<IEventHubReceiveEndpointConfigurator> _configure;
        readonly string _consumerGroup;
        readonly ReceiveEndpointObservable _endpointObservers;
        readonly string _eventHubName;
        readonly IEventHubHostConfiguration _hostConfiguration;
        readonly IHostSettings _hostSettings;
        readonly IStorageSettings _storageSettings;

        public EventHubReceiveEndpointSpecification(IEventHubHostConfiguration hostConfiguration, string eventHubName, string consumerGroup,
            IHostSettings hostSettings, IStorageSettings storageSettings,
            Action<IEventHubReceiveEndpointConfigurator> configure)
        {
            _hostConfiguration = hostConfiguration;
            _eventHubName = eventHubName;
            _consumerGroup = consumerGroup;
            _hostSettings = hostSettings;
            _storageSettings = storageSettings;
            _configure = configure;
            EndpointName = $"{EventHubEndpointAddress.PathPrefix}/{_eventHubName}";

            _endpointObservers = new ReceiveEndpointObservable();
        }

        public string EndpointName { get; }

        public ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _endpointObservers.Connect(observer);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrWhiteSpace(_eventHubName))
                yield return this.Failure("EventHubName", "should not be empty");

            if (string.IsNullOrWhiteSpace(_consumerGroup))
                yield return this.Failure("ConsumerGroup", "should not be empty");

            if (string.IsNullOrWhiteSpace(_hostSettings.ConnectionString)
                && (string.IsNullOrWhiteSpace(_hostSettings.FullyQualifiedNamespace) || _hostSettings.TokenCredential == null))
                yield return this.Failure("HostSettings", "is invalid");


            if (string.IsNullOrWhiteSpace(_storageSettings.ConnectionString) && _storageSettings.ContainerUri == null)
                yield return this.Failure("StorageSettings", "is invalid");
        }

        public ReceiveEndpoint CreateReceiveEndpoint(IBusInstance busInstance)
        {
            var endpointConfiguration = busInstance.HostConfiguration.CreateReceiveEndpointConfiguration(EndpointName);
            endpointConfiguration.ConnectReceiveEndpointObserver(_endpointObservers);

            var configurator = new EventHubReceiveEndpointConfigurator(_hostConfiguration, busInstance, endpointConfiguration, _eventHubName, _consumerGroup);
            _configure?.Invoke(configurator);

            IReadOnlyList<ValidationResult> result = Validate().Concat(configurator.Validate())
                .ThrowIfContainsFailure($"{TypeCache.GetShortName(GetType())} configuration is invalid:");

            try
            {
                return configurator.Build();
            }
            catch (Exception ex)
            {
                throw new ConfigurationException(result, "An exception occurred creating EventHub receive endpoint", ex);
            }
        }
    }
}
