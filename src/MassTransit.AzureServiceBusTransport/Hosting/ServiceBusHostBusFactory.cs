// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied. See the License for the
// specific language governing permissions and limitations under the License.
namespace MassTransit.AzureServiceBusTransport.Hosting
{
    using System;
    using Context;
    using MassTransit.Hosting;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using Microsoft.ServiceBus.Messaging.Amqp;


    public class ServiceBusHostBusFactory :
        IHostBusFactory
    {
        readonly ServiceBusAmqpTransportSettings _ampAmqpTransportSettings;

        readonly ServiceBusNetMessagingTransportSettings _netMessagingTransportSettings;
        readonly ServiceBusSettings _settings;

        public ServiceBusHostBusFactory(ISettingsProvider settingsProvider)
        {
            ServiceBusSettings settings;
            if (!settingsProvider.TryGetSettings("ServiceBus", out settings))
                throw new ConfigurationException("The ServiceBus settings were not available");

            _settings = settings;

            ServiceBusAmqpTransportSettings amqpTransportSettings;
            if (!settingsProvider.TryGetSettings("ServiceBusAmqpTransport", out amqpTransportSettings))
                throw new ConfigurationException("The ServiceBusAmqpTransport settings were not available");
            _ampAmqpTransportSettings = amqpTransportSettings;

            ServiceBusNetMessagingTransportSettings netMessagingTransportSettings;
            if (!settingsProvider.TryGetSettings("ServiceBusNetMessagingTransport", out netMessagingTransportSettings))
                throw new ConfigurationException("The ServiceBusNetMessagingTransport settings were not available");
            _netMessagingTransportSettings = netMessagingTransportSettings;
        }

        public IBusControl CreateBus(IBusServiceConfigurator busServiceConfigurator, string serviceName)
        {
            serviceName = serviceName.ToLowerInvariant().Trim().Replace(" ", "_");

            var hostSettings = new SettingsAdapter(_settings, _ampAmqpTransportSettings, _netMessagingTransportSettings, serviceName);

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
            private readonly ServiceBusAmqpTransportSettings _ampAmqpTransportSettings;

            private readonly ServiceBusNetMessagingTransportSettings _netMessagingTransportSettings;
            private readonly ServiceBusSettings _settings;

            public SettingsAdapter(ServiceBusSettings settings, ServiceBusAmqpTransportSettings ampAmqpTransportSettings,
                ServiceBusNetMessagingTransportSettings netMessagingTransportSettings, string serviceName)
            {
                _settings = settings;
                _ampAmqpTransportSettings = ampAmqpTransportSettings;
                _netMessagingTransportSettings = netMessagingTransportSettings;

                if (string.IsNullOrWhiteSpace(settings.ConnectionString))
                {
                    if (string.IsNullOrWhiteSpace(_settings.Namespace))
                        throw new ConfigurationException("The ServiceBus Namespace setting has not been configured");
                    if (string.IsNullOrEmpty(settings.KeyName))
                        throw new ConfigurationException("The ServiceBus KeyName setting has not been configured");
                    if (string.IsNullOrEmpty(settings.SharedAccessKey))
                        throw new ConfigurationException("The ServiceBus SharedAccessKey setting has not been configured");

                    ServiceUri = ServiceBusEnvironment.CreateServiceUri("sb", settings.Namespace, settings.ServicePath ?? serviceName);
                    TokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(settings.KeyName, settings.SharedAccessKey);
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
            public TimeSpan OperationTimeout => _settings.OperationTimeout ?? TimeSpan.FromSeconds(30);

            public TimeSpan RetryMinBackoff => _settings.RetryMinBackoff ?? TimeSpan.Zero;

            public TimeSpan RetryMaxBackoff => _settings.RetryMaxBackoff ?? TimeSpan.FromSeconds(2);

            public int RetryLimit => _settings.RetryLimit ?? 10;

            public TransportType TransportType => _settings.TransportType;

            public AmqpTransportSettings AmqpTransportSettings
            {
                get
                {
                    // Leave AmqpTransportSettings default values intact unless configuration value provided.
                    var settings = new AmqpTransportSettings();

                    if (_ampAmqpTransportSettings.BatchFlushInterval.HasValue)
                        settings.BatchFlushInterval = _ampAmqpTransportSettings.BatchFlushInterval.Value;

                    if (_ampAmqpTransportSettings.EnableLinkRedirect.HasValue)
                        settings.EnableLinkRedirect = _ampAmqpTransportSettings.EnableLinkRedirect.Value;

                    if (_ampAmqpTransportSettings.MaxFrameSize.HasValue)
                        settings.MaxFrameSize = _ampAmqpTransportSettings.MaxFrameSize.Value;

                    if (_ampAmqpTransportSettings.UseSslStreamSecurity.HasValue)
                        settings.UseSslStreamSecurity = _ampAmqpTransportSettings.UseSslStreamSecurity.Value;

                    return settings;
                }
            }

            public NetMessagingTransportSettings NetMessagingTransportSettings
            {
                get
                {
                    // Leave NetMessagingTransportSettings default values intact unless configuration value provided.
                    var settings = new NetMessagingTransportSettings();
                    if (_netMessagingTransportSettings.BatchFlushInterval.HasValue)
                        settings.BatchFlushInterval = _netMessagingTransportSettings.BatchFlushInterval.Value;

                    if (_netMessagingTransportSettings.EnableRedirect.HasValue)
                        settings.EnableRedirect = _netMessagingTransportSettings.EnableRedirect.Value;

                    if (_netMessagingTransportSettings.LeaseTimeout.HasValue)
                        settings.LeaseTimeout = _netMessagingTransportSettings.LeaseTimeout.Value;

                    return settings;
                }
            }

            public Uri ServiceUri { get; private set; }

            public TokenProvider TokenProvider { get; private set; }
        }
    }
}
