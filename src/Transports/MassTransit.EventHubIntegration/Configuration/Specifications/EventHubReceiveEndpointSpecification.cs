namespace MassTransit.EventHubIntegration.Specifications
{
    using System;
    using System.Collections.Generic;
    using Configurators;
    using GreenPipes;
    using MassTransit.Configurators;
    using MassTransit.Registration;
    using Pipeline.Observables;


    public class EventHubReceiveEndpointSpecification :
        IEventHubReceiveEndpointSpecification
    {
        readonly Action<IEventHubReceiveEndpointConfigurator> _configure;
        readonly string _consumerGroup;
        readonly ReceiveEndpointObservable _endpointObservers;
        readonly IHostSettings _hostSettings;
        readonly IStorageSettings _storageSettings;

        public EventHubReceiveEndpointSpecification(string eventHubName, string consumerGroup, IHostSettings hostSettings, IStorageSettings storageSettings,
            Action<IEventHubReceiveEndpointConfigurator> configure)
        {
            _consumerGroup = consumerGroup;
            _hostSettings = hostSettings;
            _storageSettings = storageSettings;
            _configure = configure;
            Name = eventHubName;

            _endpointObservers = new ReceiveEndpointObservable();
        }

        public string Name { get; }

        public ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _endpointObservers.Connect(observer);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
                yield return this.Failure("EventHubName", "should not be empty");

            if (string.IsNullOrWhiteSpace(_consumerGroup))
                yield return this.Failure("ConsumerGroup", "should not be empty");

            if (string.IsNullOrWhiteSpace(_hostSettings.ConnectionString)
                && (string.IsNullOrWhiteSpace(_hostSettings.FullyQualifiedNamespace) || _hostSettings.TokenCredential == null))
                yield return this.Failure("HostSettings", "is invalid");


            if (string.IsNullOrWhiteSpace(_storageSettings.ConnectionString) && _storageSettings.ContainerUri == null)
                yield return this.Failure("StorageSettings", "is invalid");
        }

        public IEventHubReceiveEndpoint Create(IBusInstance busInstance)
        {
            var endpointConfiguration = busInstance.HostConfiguration.CreateReceiveEndpointConfiguration($"{EventHubEndpointAddress.PathPrefix}/{Name}");
            endpointConfiguration.ConnectReceiveEndpointObserver(_endpointObservers);

            var configurator =
                new EventHubReceiveEndpointConfigurator(Name, _consumerGroup, _hostSettings, _storageSettings, busInstance, endpointConfiguration);
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
    }
}
