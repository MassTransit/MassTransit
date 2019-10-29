// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Linq;
    using Apache.NMS;


    public class ConfigurationHostSettings :
        ActiveMqHostSettings
    {
        readonly Lazy<Uri> _brokerAddress;
        readonly Lazy<Uri> _hostAddress;

        public ConfigurationHostSettings()
        {
            _hostAddress = new Lazy<Uri>(FormatHostAddress);
            _brokerAddress = new Lazy<Uri>(FormatBrokerAddress);
        }

        public string[] FailoverHosts { get; set; }
        public Dictionary<string, string> TransportOptions { get; set; }

        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool UseSsl { get; set; }

        public Uri HostAddress => _hostAddress.Value;
        public Uri BrokerAddress => _brokerAddress.Value;

        public IConnection CreateConnection()
        {
            var factory = new NMSConnectionFactory(BrokerAddress);
            return factory.CreateConnection(Username, Password);
        }

        Uri FormatHostAddress()
        {
            return new ActiveMqHostAddress(Host, Port, "/");
        }

        Uri FormatBrokerAddress()
        {
            var scheme = UseSsl ? "ssl" : "tcp";
            var queryPart = GetQueryString();

            // create broker URI: http://activemq.apache.org/nms/activemq-uri-configuration.html
            if (FailoverHosts?.Length > 0)
            {
                var failoverPart = string.Join(",", FailoverHosts
                    .Select(failoverHost => new UriBuilder
                        {
                            Scheme = scheme,
                            Host = failoverHost,
                            Port = Port
                        }.Uri.ToString()
                    ));


                return new Uri($"activemq:failover:({failoverPart}){queryPart}");
            }


            var uri = new Uri($"activemq:{scheme}://{Host}:{Port}{queryPart}");
            return uri;
        }

        string GetQueryString()
        {
            var queryPart = string.Empty;
            if (TransportOptions?.Count > 0)
            {
                var queryString = string.Join("&", TransportOptions
                    .Select(pair => $"{pair.Key}={pair.Value}"));

                queryString = $"?{queryString}";
            }

            return queryPart;
        }

        public override string ToString()
        {
            return new UriBuilder
            {
                Scheme = UseSsl ? "ssl" : "tcp",
                Host = Host,
                Port = Port
            }.Uri.ToString();
        }
    }
}
