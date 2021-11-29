namespace MassTransit.Configuration
{
    using System;
    using Azure;
    using Azure.Core;
    using Azure.Messaging.ServiceBus;
    using Azure.Messaging.ServiceBus.Administration;
    using AzureServiceBusTransport.Configuration;


    public class ServiceBusHostConfigurator :
        IServiceBusHostConfigurator
    {
        readonly HostSettings _settings;

        public ServiceBusHostConfigurator(Uri serviceAddress)
        {
            var hostAddress = new ServiceBusHostAddress(serviceAddress);

            _settings = new HostSettings { ServiceUri = hostAddress };
        }

        public ServiceBusHostConfigurator(Uri serviceAddress,
            ServiceBusClient serviceBusClient,
            ServiceBusAdministrationClient serviceBusAdministrationClient)
        {
            var hostAddress = new ServiceBusHostAddress(serviceAddress);

            _settings = new HostSettings
            {
                ServiceUri = hostAddress,
                ServiceBusClient = serviceBusClient ?? throw new ArgumentNullException(nameof(serviceBusClient)),
                ServiceBusAdministrationClient = serviceBusAdministrationClient ?? throw new ArgumentNullException(nameof(serviceBusAdministrationClient)),
            };
        }

        public ServiceBusHostConfigurator(string connectionString)
        {
            var properties = ServiceBusConnectionStringProperties.Parse(connectionString);

            _settings = new HostSettings
            {
                ConnectionString = connectionString,
                ServiceUri = properties.Endpoint,
            };

            if (IsMissingCredentials(properties))
                _settings.ConnectionString = null;
        }

        public ServiceBusHostSettings Settings => _settings;

        public string ConnectionString
        {
            set
            {
                if (_settings.NamedKeyCredential != null
                    || _settings.SasCredential != null
                    || _settings.TokenCredential != null)
                    throw new ArgumentException("Another type of authentication is already being used");

                _settings.ConnectionString = value;
            }
        }

        public AzureNamedKeyCredential NamedKeyCredential
        {
            set
            {
                if (_settings.ConnectionString != null
                    || _settings.SasCredential != null
                    || _settings.TokenCredential != null)
                    throw new ArgumentException("Another type of authentication is already being used");

                _settings.NamedKeyCredential = value;
            }
        }

        public AzureSasCredential SasCredential
        {
            set
            {
                if (_settings.ConnectionString != null
                    || _settings.NamedKeyCredential != null
                    || _settings.TokenCredential != null)
                    throw new ArgumentException("Another type of authentication is already being used");

                _settings.SasCredential = value;
            }
        }

        public TokenCredential TokenCredential
        {
            set
            {
                if (_settings.ConnectionString != null
                    || _settings.SasCredential != null
                    || _settings.NamedKeyCredential != null)
                    throw new ArgumentException("Another type of authentication is already being used");

                _settings.TokenCredential = value;
            }
        }

        public ServiceBusTransportType TransportType
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

        static bool IsMissingCredentials(ServiceBusConnectionStringProperties properties)
        {
            return string.IsNullOrWhiteSpace(properties.SharedAccessKeyName) && string.IsNullOrWhiteSpace(properties.SharedAccessKey)
                && string.IsNullOrWhiteSpace(properties.SharedAccessSignature);
        }
    }
}
