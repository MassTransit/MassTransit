namespace MassTransit.EventHubIntegration.Processors
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using Configurators;
    using GreenPipes;
    using Pipeline.Observables;
    using Registration;


    public class EventHubDefinition :
        IEventHubDefinition
    {
        readonly Action<IEventHubConfigurator> _configure;
        readonly string _consumerGroup;
        readonly ReceiveEndpointObservable _endpointObservers;
        readonly IHostSettings _hostSettings;
        readonly IStorageSettings _storageSettings;

        public EventHubDefinition(string eventHubName, string consumerGroup, IHostSettings hostSettings, IStorageSettings storageSettings,
            Action<IEventHubConfigurator> configure)
        {
            _consumerGroup = consumerGroup;
            _hostSettings = hostSettings;
            _storageSettings = storageSettings;
            _configure = configure;
            Name = eventHubName;

            _endpointObservers = new ReceiveEndpointObservable();
        }

        public string Name { get; }

        public IEventHubReceiveEndpoint Create(IBusInstance busInstance)
        {
            var endpointConfiguration = busInstance.HostConfiguration.CreateReceiveEndpointConfiguration($"event-hub/{Name}");
            endpointConfiguration.ConnectReceiveEndpointObserver(_endpointObservers);
            var configurator =
                new EventHubConfigurator(Name, _consumerGroup, _hostSettings, _storageSettings, busInstance, endpointConfiguration);
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
    }
}
