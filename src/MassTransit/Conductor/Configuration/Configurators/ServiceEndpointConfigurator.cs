namespace MassTransit.Conductor.Configuration.Configurators
{
    using System.Collections.Generic;
    using EndpointConfigurators;
    using GreenPipes;
    using Observers;
    using Server;


    public class ServiceEndpointConfigurator :
        IServiceEndpointConfigurator,
        IEndpointConfigurationObserver
    {
        readonly IList<ConnectHandle> _configurationHandles;
        readonly IReceiveEndpointConfigurator _serviceEndpointConfigurator;
        readonly IReceiveEndpointConfigurator _controlEndpointConfigurator;
        readonly IServiceEndpoint _endpoint;

        public ServiceEndpointConfigurator(IServiceInstance instance, IEndpointConfigurationObserverConnector connector,
            IReceiveEndpointConfigurator serviceEndpointConfigurator, IReceiveEndpointConfigurator controlEndpointConfigurator = null)
        {
            _serviceEndpointConfigurator = serviceEndpointConfigurator;
            _controlEndpointConfigurator = controlEndpointConfigurator;

            _configurationHandles = new List<ConnectHandle>();

            _endpoint = instance.CreateServiceEndpoint(serviceEndpointConfigurator);

            ConnectConfigurationObserver(serviceEndpointConfigurator);

            _configurationHandles.Add(connector.ConnectEndpointConfigurationObserver(this));
        }

        void ConnectConfigurationObserver(IConsumePipeConfigurator configurator)
        {
            var configurationObserver = new ServiceEndpointConfigurationObserver(configurator, this);

            _configurationHandles.Add(configurator.ConnectConsumerConfigurationObserver(configurationObserver));
            _configurationHandles.Add(configurator.ConnectHandlerConfigurationObserver(configurationObserver));
            _configurationHandles.Add(configurator.ConnectSagaConfigurationObserver(configurationObserver));
            _configurationHandles.Add(configurator.ConnectActivityConfigurationObserver(configurationObserver));
        }

        void DisconnectConfigurationObservers()
        {
            foreach (var handle in _configurationHandles)
            {
                handle.Disconnect();
            }

            _configurationHandles.Clear();
        }

        public void ConfigureMessage<T>(IConsumePipeConfigurator configurator)
            where T : class
        {
            _endpoint.ConfigureServiceEndpoint<T>(configurator);

            if (_controlEndpointConfigurator != null)
                _endpoint.ConfigureControlEndpoint<T>(_controlEndpointConfigurator);
        }

        public void EndpointConfigured<T>(T configurator)
            where T : IReceiveEndpointConfigurator
        {
            if (ReferenceEquals(configurator, _serviceEndpointConfigurator))
                DisconnectConfigurationObservers();
        }
    }
}
