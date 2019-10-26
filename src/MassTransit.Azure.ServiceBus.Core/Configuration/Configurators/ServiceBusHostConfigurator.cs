namespace MassTransit.Azure.ServiceBus.Core.Configurators
{
    using System;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Primitives;


    public class ServiceBusHostConfigurator :
        IServiceBusHostConfigurator
    {
        readonly HostSettings _settings;

        public ServiceBusHostConfigurator(Uri serviceAddress)
        {
            var hostAddress = new ServiceBusHostAddress(serviceAddress);

            _settings = new HostSettings {ServiceUri = hostAddress};
        }

        public ServiceBusHostSettings Settings => _settings;

        public ITokenProvider TokenProvider
        {
            set => _settings.TokenProvider = value;
        }

        public TimeSpan OperationTimeout
        {
            set => _settings.OperationTimeout = value;
        }

        public TransportType TransportType
        {
            set => _settings.TransportType = value;
        }

        public TimeSpan RetryMinBackoff
        {
            set => _settings.RetryMinBackoff = value;
        }

        public TimeSpan RetryMaxBackoff
        {
            set => _settings.RetryMaxBackoff = value;
        }

        public int RetryLimit
        {
            set => _settings.RetryLimit = value;
        }
    }
}
