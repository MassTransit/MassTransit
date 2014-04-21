// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit
{
    using System;
    using Transports.RabbitMq.Configuration.Configurators;


    public class RabbitMqHostConfiguratorImpl :
        RabbitMqHostConfigurator
    {
        static readonly char[] _pathSeparator = {'/'};
        readonly HostSettings _settings;

        public RabbitMqHostConfiguratorImpl(Uri hostAddress)
        {
            _settings = new HostSettings
            {
                Host = hostAddress.Host,
                Port = hostAddress.IsDefaultPort ? 5672 : hostAddress.Port,
                VirtualHost = GetVirtualHost(hostAddress)
            };
        }

        public RabbitMqHostSettings Settings
        {
            get { return _settings; }
        }

        public void UseSsl(Action<SslConnectionFactoryConfigurator> configureSsl)
        {
            throw new NotImplementedException("Coming soon, surely");
        }

        public void Heartbeat(ushort requestedHeartbeat)
        {
            _settings.Heartbeat = requestedHeartbeat;
        }

        public void Username(string username)
        {
            _settings.Username = username;
        }

        public void Password(string password)
        {
            _settings.Password = password;
        }

        string GetVirtualHost(Uri address)
        {
            string[] segments = address.AbsolutePath.Split(_pathSeparator, StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length == 0)
                return null;
            if (segments.Length == 1)
                return segments[0];

            throw new FormatException("The host path must be empty or contain a single virtual host name");
        }


        class HostSettings :
            RabbitMqHostSettings
        {
            public string Host { get; set; }
            public int Port { get; set; }
            public string VirtualHost { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public ushort Heartbeat { get; set; }
        }
    }
}