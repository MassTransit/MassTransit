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
namespace MassTransit.RabbitMqTransport.Hosting
{
    using System.Net.Security;
    using System.Security.Authentication;
    using System.Security.Cryptography.X509Certificates;
    using Logging;
    using MassTransit.Hosting;
    using Transports;


    public class RabbitMqHostBusFactory :
        IHostBusFactory
    {
        readonly ILog _log = Logger.Get<RabbitMqHostBusFactory>();

        readonly SettingsAdapter _hostSettings;

        public RabbitMqHostBusFactory(ISettingsProvider settingsProvider)
        {
            RabbitMqSettings settings;
            if (!settingsProvider.TryGetSettings("RabbitMQ", out settings))
                throw new ConfigurationException("The RabbitMQ settings were not available");

            _hostSettings = new SettingsAdapter(settings);
        }

        public IBusControl CreateBus(IBusServiceConfigurator busServiceConfigurator, string serviceName)
        {
            return RabbitMqBusFactory.Create(configurator =>
            {
                var host = configurator.Host(_hostSettings);

                if(_log.IsInfoEnabled)
                    _log.Info($"Configuring Host: {_hostSettings.ToDebugString()}");

                var serviceConfigurator = new RabbitMqServiceConfigurator(configurator, host);

                busServiceConfigurator.Configure(serviceConfigurator);
            });
        }


        class SettingsAdapter :
            RabbitMqHostSettings
        {
            readonly RabbitMqSettings _settings;
            RabbitMqHostSettings _rabbitMqHostSettingsImplementation;

            public SettingsAdapter(RabbitMqSettings settings)
            {
                _settings = settings;
            }

            public string Host => _settings.Host ?? "[::1]";
            public int Port => _settings.Port ?? 5672;
            public string VirtualHost => _settings.VirtualHost ?? "/";
            public string Username => _settings.Username ?? "guest";
            public string Password => _settings.Password ?? "guest";
            public ushort Heartbeat => _settings.Heartbeat ?? 0;
            public bool Ssl => false;
            public SslProtocols SslProtocol => SslProtocols.None;
            public string SslServerName => null;
            public SslPolicyErrors AcceptablePolicyErrors => SslPolicyErrors.None;
            public string ClientCertificatePath => null;
            public string ClientCertificatePassphrase => null;
            public X509Certificate ClientCertificate => null;
            public bool UseClientCertificateAsAuthenticationIdentity => false;
            public IMessageNameFormatter MessageNameFormatter => new RabbitMqMessageNameFormatter();
            public string[] ClusterMembers => null;
            public IRabbitMqHostNameSelector HostNameSelector => _rabbitMqHostSettingsImplementation.HostNameSelector;
        }
    }
}