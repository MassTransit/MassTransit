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
namespace MassTransit.ActiveMqTransport.Configurators
{
    using System;
    using System.Collections.Generic;


    public class ActiveMqHostConfigurator :
        IActiveMqHostConfigurator
    {
        readonly ConfigurationHostSettings _settings;

        public ActiveMqHostConfigurator(Uri address)
        {
            var hostAddress = new ActiveMqHostAddress(address);

            _settings = new ConfigurationHostSettings
            {
                Host = hostAddress.Host,
                Username = "",
                Password = "",
                TransportOptions = new Dictionary<string, string>()
                {
                    { "wireFormat.tightEncodingEnabled", "true" },
                    { "nms.AsyncSend", "true" }
                }
            };

            _settings.Port = hostAddress.Port ?? 61616;

            if (!string.IsNullOrEmpty(address.UserInfo))
            {
                string[] parts = address.UserInfo.Split(':');
                _settings.Username = parts[0];

                if (parts.Length >= 2)
                    _settings.Password = parts[1];
            }
        }

        public ActiveMqHostSettings Settings => _settings;

        public void Username(string username)
        {
            _settings.Username = username;
        }

        public void Password(string password)
        {
            _settings.Password = password;
        }

        public void UseSsl()
        {
            _settings.UseSsl = true;
            if (_settings.Port == 61616)
                _settings.Port = 61617;
        }

        public void FailoverHosts(string[] hosts)
        {
            _settings.FailoverHosts = hosts;
        }

        public void TransportOptions(Dictionary<string, string> options)
        {
            foreach (KeyValuePair<string, string> option in options)
            {
                _settings.TransportOptions[option.Key] = option.Value;
            }
        }
    }
}
