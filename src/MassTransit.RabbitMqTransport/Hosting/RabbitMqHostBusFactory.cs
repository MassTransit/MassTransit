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
namespace MassTransit.RabbitMqTransport.Hosting
{
    using Configurators;
    using Logging;
    using MassTransit.Hosting;


    public class RabbitMqHostBusFactory :
        IHostBusFactory
    {
        readonly RabbitMqHostSettings _hostSettings;
        readonly ILog _log = Logger.Get<RabbitMqHostBusFactory>();

        public RabbitMqHostBusFactory(ISettingsProvider settingsProvider)
        {
            if (!settingsProvider.TryGetSettings("RabbitMQ", out RabbitMqSettings settings))
                throw new ConfigurationException("The RabbitMQ settings were not available");

            _hostSettings = new ConfigurationHostSettings
            {
                Host = settings.Host ?? "[::1]",
                Port = settings.Port ?? 5672,
                VirtualHost = string.IsNullOrWhiteSpace(settings.VirtualHost) ? "/" : settings.VirtualHost.Trim('/'),
                Username = settings.Username ?? "guest",
                Password = settings.Password ?? "guest",
                Heartbeat = settings.Heartbeat ?? 0,
                ClusterMembers = settings.ClusterMembers?.Split(',')
            };
        }

        public IBusControl CreateBus(IBusServiceConfigurator busServiceConfigurator, string serviceName)
        {
            return RabbitMqBusFactory.Create(configurator =>
            {
                var host = configurator.Host(_hostSettings);

                if (_log.IsInfoEnabled)
                    _log.Info($"Configuring Host: {_hostSettings.ToDebugString()}");

                var serviceConfigurator = new RabbitMqServiceConfigurator(configurator, host);

                busServiceConfigurator.Configure(serviceConfigurator);
            });
        }
    }
}