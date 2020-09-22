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

        public ServiceBusHostConfigurator(string connectionString)
        {
            var builder = new ServiceBusConnectionStringBuilder(connectionString);

            var serviceUri = GetFullEndpointUri(connectionString);

            _settings = new HostSettings
            {
                ServiceUri = serviceUri,
                OperationTimeout = builder.OperationTimeout,
                TransportType = builder.TransportType
            };

            if (builder.SasToken != null)
                _settings.TokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(builder.SasToken);
            else if (builder.SasKeyName != null && builder.SasKey != null)
                _settings.TokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(builder.SasKeyName, builder.SasKey);
            else
                throw new Exception("The connection string did not contain an SAS token or an SAS key name and SAS key pair.");
        }

        public ServiceBusHostSettings Settings => _settings;

        ITokenProvider IServiceBusHostConfigurator.TokenProvider
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

        // Code copied from: https://github.com/Azure/azure-service-bus-dotnet/blob/8f619194132faab19507811f38137a9a8582f488/src/Microsoft.Azure.ServiceBus/ServiceBusConnectionStringBuilder.cs#L294
        private Uri GetFullEndpointUri(string connectionString) {
            var keyValuePairs = connectionString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var keyValuePair in keyValuePairs)
            {
                // Now split based on the _first_ '='
                var keyAndValue = keyValuePair.Split(new[] { '=' }, 2);
                var key = keyAndValue[0];
                if (keyAndValue.Length != 2)
                {
                    throw new UriFormatException($"Value for the connection string parameter name '{key}' was not found.");
                }

                var value = keyAndValue[1].Trim();
                if (key.Equals("Endpoint", StringComparison.OrdinalIgnoreCase))
                {
                    return new Uri(value);
                }
            }
            return null;
        }
    }
}
