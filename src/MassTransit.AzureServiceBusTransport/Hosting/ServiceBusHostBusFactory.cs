// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Logging;
    using MassTransit.Hosting;
    using Microsoft.ServiceBus;


    public class ServiceBusHostBusFactory :
        IHostBusFactory
    {
        readonly ILog _log = Logger.Get<ServiceBusHostBusFactory>();
        ServiceBusSettings _settings;

        public ServiceBusHostBusFactory(ISettingsProvider settingsProvider)
        {
            ServiceBusSettings settings;
            if (!settingsProvider.TryGetSettings("ServiceBus", out settings))
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
                    h.SharedAccessSignature(s =>
                    {
                        s.KeyName = hostSettings.KeyName;
                        s.SharedAccessKey = hostSettings.SharedAccessKey;
                        s.TokenTimeToLive = hostSettings.TokenTimeToLive;
                        s.TokenScope = hostSettings.TokenScope;
                    });
                });

                if (_log.IsInfoEnabled)
                    _log.Info($"Configuring Host: {hostSettings.ServiceUri}");

                var serviceConfigurator = new ServiceBusServiceConfigurator(configurator, host);

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

            public string KeyName => _settings.KeyName;
            public string SharedAccessKey => _settings.SharedAccessKey;
            public TimeSpan TokenTimeToLive => _settings.TokenTimeToLive ?? TimeSpan.FromDays(1);
            public TokenScope TokenScope => _settings.TokenScope;
            public TimeSpan OperationTimeout => _settings.OperationTimeout ?? TimeSpan.FromSeconds(30);

            public TimeSpan RetryMinBackoff => _settings.RetryMinBackoff ?? TimeSpan.Zero;

            public TimeSpan RetryMaxBackoff => _settings.RetryMaxBackoff ?? TimeSpan.FromSeconds(2);

            public int RetryLimit => _settings.RetryLimit ?? 10;

            public Uri ServiceUri { get; private set; }

            public TokenProvider TokenProvider { get; private set; }
        }
    }
}