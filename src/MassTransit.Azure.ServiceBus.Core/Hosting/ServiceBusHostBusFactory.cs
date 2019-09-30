namespace MassTransit.Azure.ServiceBus.Core.Hosting
{
    using System;
    using Context;
    using Contexts;
    using MassTransit.Hosting;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Primitives;


    public class ServiceBusHostBusFactory :
        IHostBusFactory
    {
        readonly ServiceBusSettings _settings;

        public ServiceBusHostBusFactory(ISettingsProvider settingsProvider)
        {
            if (!settingsProvider.TryGetSettings("ServiceBus", out ServiceBusSettings settings))
                throw new ConfigurationException("The ServiceBus settings were not available");

            _settings = settings;
        }

        public IBusControl CreateBus(IBusServiceConfigurator busServiceConfigurator, string serviceName)
        {
            serviceName = serviceName.ToLowerInvariant().Trim().Replace(" ", "_");

            var hostSettings = new SettingsAdapter(_settings, serviceName);

            if (hostSettings.ServiceUri == null)
                throw new ConfigurationException("The ServiceBus ServiceUri setting has not been configured");

            return AzureBusFactory.CreateUsingServiceBus(configurator =>
            {
                var host = configurator.Host(hostSettings.ServiceUri, h =>
                {
                    if (!string.IsNullOrWhiteSpace(hostSettings.ConnectionString))
                    {
                        h.TokenProvider = hostSettings.TokenProvider;
                    }
                    else
                    {
                        h.SharedAccessSignature(s =>
                        {
                            s.KeyName = hostSettings.KeyName;
                            s.SharedAccessKey = hostSettings.SharedAccessKey;
                            s.TokenTimeToLive = hostSettings.TokenTimeToLive;
                            s.TokenScope = hostSettings.TokenScope;
                        });
                    }
                });

                LogContext.Info?.Log("Configuring Host: {Host}", hostSettings.ServiceUri);

                var serviceConfigurator = new ServiceBusServiceConfigurator(configurator);

                busServiceConfigurator.Configure(serviceConfigurator);
            });
        }


        class SettingsAdapter :
            ServiceBusHostSettings
        {
            readonly ServiceBusSettings _settings;

            public SettingsAdapter(ServiceBusSettings settings, string serviceName)
            {
                _settings = settings;

                if (string.IsNullOrWhiteSpace(settings.ConnectionString))
                {
                    if (string.IsNullOrWhiteSpace(_settings.Namespace))
                        throw new ConfigurationException("The ServiceBus Namespace setting has not been configured");
                    if (string.IsNullOrEmpty(settings.KeyName))
                        throw new ConfigurationException("The ServiceBus KeyName setting has not been configured");
                    if (string.IsNullOrEmpty(settings.SharedAccessKey))
                        throw new ConfigurationException("The ServiceBus SharedAccessKey setting has not been configured");

                    ServiceUri = AzureServiceBusEndpointUriCreator.Create(_settings.Namespace, _settings.ServicePath ?? serviceName);
                    TokenProvider = Microsoft.Azure.ServiceBus.Primitives.TokenProvider.CreateSharedAccessSignatureTokenProvider(settings.KeyName,
                        settings.SharedAccessKey);
                }
                else
                {
                    var namespaceManager = NamespaceManager.CreateFromConnectionString(settings.ConnectionString);

                    ServiceUri = namespaceManager.Address;
                    TokenProvider = namespaceManager.Settings.TokenProvider;
                }
            }

            public string ConnectionString => _settings.ConnectionString;

            public string KeyName => _settings.KeyName;
            public string SharedAccessKey => _settings.SharedAccessKey;
            public TimeSpan TokenTimeToLive => _settings.TokenTimeToLive ?? TimeSpan.FromDays(1);
            public TokenScope TokenScope => _settings.TokenScope;
            public TimeSpan OperationTimeout => _settings.OperationTimeout ?? TimeSpan.FromSeconds(60);

            public TimeSpan RetryMinBackoff => _settings.RetryMinBackoff ?? TimeSpan.Zero;

            public TimeSpan RetryMaxBackoff => _settings.RetryMaxBackoff ?? TimeSpan.FromSeconds(2);

            public int RetryLimit => _settings.RetryLimit ?? 10;

            public TransportType TransportType => _settings.TransportType;

            public Uri ServiceUri { get; private set; }

            public ITokenProvider TokenProvider { get; private set; }
        }
    }
}
