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

        readonly SettingsAdapter _hostSettings;

        public ServiceBusHostBusFactory(ISettingsProvider settingsProvider)
        {
            ServiceBusSettings settings;
            if (!settingsProvider.TryGetSettings("ServiceBus", out settings))
                throw new ConfigurationException("The ServiceBus settings were not available");

            _hostSettings = new SettingsAdapter(settings);
        }

        public IBusControl CreateBus(IBusServiceConfigurator busServiceConfigurator, string serviceName)
        {
            if (string.IsNullOrEmpty(_hostSettings.Namespace))
                throw new ConfigurationException("The ServiceBus Namespace setting has not been configured");

            if (string.IsNullOrEmpty(_hostSettings.KeyName))
                throw new ConfigurationException("The ServiceBus KeyName setting has not been configured");

            if (string.IsNullOrEmpty(_hostSettings.SharedAccessKey))
                throw new ConfigurationException("The ServiceBus SharedAccessKey setting has not been configured");

            return AzureBusFactory.CreateUsingServiceBus(configurator =>
            {
                serviceName = serviceName.ToLowerInvariant().Trim().Replace(" ", "_");
                _hostSettings.ServiceUri = ServiceBusEnvironment.CreateServiceUri("sb",
                    _hostSettings.Namespace, serviceName);
                
                var host = configurator.Host(_hostSettings.ServiceUri, h =>
                {
                    h.SharedAccessSignature(s =>
                    {
                        s.KeyName = _hostSettings.KeyName;
                        s.SharedAccessKey = _hostSettings.SharedAccessKey;
                        s.TokenTimeToLive = _hostSettings.TokenTimeToLive;
                        s.TokenScope = _hostSettings.TokenScope;
                    });
                });

                if (_log.IsInfoEnabled)
                    _log.Info($"Configuring Host: { _hostSettings.ServiceUri}");

                var serviceConfigurator = new ServiceBusServiceConfigurator(configurator, host);

                busServiceConfigurator.Configure(serviceConfigurator);
            });
        }


        class SettingsAdapter :
            ServiceBusHostSettings
        {
            readonly ServiceBusSettings _settings;

            public SettingsAdapter(ServiceBusSettings settings)
            {
                _settings = settings;
            }

            public string Namespace => _settings.Namespace;
            public string KeyName => _settings.KeyName;
            public string SharedAccessKey => _settings.SharedAccessKey;
            public TimeSpan TokenTimeToLive => _settings.TokenTimeToLive ?? TimeSpan.FromDays(1);
            public TokenScope TokenScope => _settings.TokenScope;
            public TimeSpan OperationTimeout => _settings.OperationTimeout ?? TimeSpan.FromSeconds(30);
            public Uri ServiceUri { get; set; }
            public TokenProvider TokenProvider { get; set; }
        }
    }
}