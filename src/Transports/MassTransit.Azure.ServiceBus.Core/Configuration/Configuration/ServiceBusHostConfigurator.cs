#nullable enable
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
                ServiceUri = ParseEndpoint(connectionString),
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

        public static Uri? ParseEndpoint(string connectionString)
        {
            var itemIndex = connectionString[0] == ';' ? 0 : 1;
            var startIndex = 0;
            var separatorIndex = 0;
            while (separatorIndex != -1)
            {
                separatorIndex = connectionString.IndexOf(';', startIndex + 1);
                var item = separatorIndex < 0 ? connectionString.Substring(startIndex) : connectionString.Substring(startIndex, separatorIndex - startIndex);
                var index = item.IndexOf('=');
                if (index >= 0)
                {
                    var key = item.Substring(1 - itemIndex, index - 1 + itemIndex);
                    var value = item.Substring(index + 1);
                    if ((!string.IsNullOrEmpty(key) && char.IsWhiteSpace(key[0])) || char.IsWhiteSpace(key[key.Length - 1]))
                        key = key.Trim();
                    if (!string.IsNullOrEmpty(value) && (char.IsWhiteSpace(value[0]) || char.IsWhiteSpace(value[value.Length - 1])))
                        value = value.Trim();
                    if (string.IsNullOrEmpty(value))
                        throw new FormatException("Invalid connection string");

                    if (string.Compare("Endpoint", key, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        if (!Uri.TryCreate(value, UriKind.Absolute, out var result))
                            result = null;

                        if ((object?)result == null)
                            return null;

                        var builder = new UriBuilder
                        {
                            Scheme = "sb",
                            Host = result.Host,
                            Path = result.AbsolutePath,
                            Port = result.IsDefaultPort ? -1 : result.Port
                        };

                        if (string.Compare(builder.Scheme, "sb", StringComparison.OrdinalIgnoreCase) != 0
                            || Uri.CheckHostName(builder.Host) == UriHostNameType.Unknown)
                            throw new FormatException("Invalid connection string");

                        return builder.Uri;
                    }
                }
                else if (item.Length != 1 || item[0] != ';')
                    throw new FormatException("Invalid connection string");

                itemIndex = 0;
                startIndex = separatorIndex;
            }

            return null;
        }
    }
}
