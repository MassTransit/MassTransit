namespace MassTransit.Azure.ServiceBus.Core.Configurators
{
    using System;
    using global::Azure;
    using global::Azure.Core;
    using global::Azure.Messaging.ServiceBus;


    public class ServiceBusHostConfigurator :
        IServiceBusHostConfigurator
    {
        readonly HostSettings _settings;

        public ServiceBusHostConfigurator(Uri serviceAddress)
        {
            var hostAddress = new ServiceBusHostAddress(serviceAddress);

            _settings = new HostSettings {ServiceUri = hostAddress};
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
            {
                _settings.ConnectionString = null;
            }
        }

        static bool IsMissingCredentials(ServiceBusConnectionStringProperties properties)
        {
            return string.IsNullOrWhiteSpace(properties.SharedAccessKeyName) && string.IsNullOrWhiteSpace(properties.SharedAccessKey) && string.IsNullOrWhiteSpace(properties.SharedAccessSignature);
        }

        public ServiceBusHostSettings Settings => _settings;

        string IServiceBusHostConfigurator.ConnectionString
        {
            set
            {
                if (_settings.NamedKeyCredential != null
                    || _settings.SasCredential != null
                    || _settings.TokenCredential != null)
                {
                    throw new ArgumentException("Another type of authentication is already being used");
                }

                _settings.ConnectionString = value;
            }
        }

        AzureNamedKeyCredential IServiceBusHostConfigurator.NamedKeyCredential
        {
            set
            {
                if (_settings.ConnectionString != null
                    || _settings.SasCredential != null
                    || _settings.TokenCredential != null)
                {
                    throw new ArgumentException("Another type of authentication is already being used");
                }

                _settings.NamedKeyCredential = value;
            }
        }

        AzureSasCredential IServiceBusHostConfigurator.SasCredential
        {
            set
            {
                if (_settings.ConnectionString != null
                    || _settings.NamedKeyCredential != null
                    || _settings.TokenCredential != null)
                {
                    throw new ArgumentException("Another type of authentication is already being used");
                }

                _settings.SasCredential = value;
            }
        }

        TokenCredential IServiceBusHostConfigurator.TokenCredential
        {
            set
            {
                if (_settings.ConnectionString != null
                    || _settings.SasCredential != null
                    || _settings.NamedKeyCredential != null)
                {
                    throw new ArgumentException("Another type of authentication is already being used");
                }

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
    }
}
