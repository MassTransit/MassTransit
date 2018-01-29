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
    using Apache.NMS;
    using Apache.NMS.ActiveMQ;


    public class ConfigurationHostSettings :
        ActiveMqHostSettings
    {
        readonly Lazy<Uri> _hostAddress;

        public ConfigurationHostSettings()
        {
            _hostAddress = new Lazy<Uri>(FormatHostAddress);
        }

        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool UseSsl { get; set; }

        public Uri HostAddress => _hostAddress.Value;

        public IConnectionFactory CreateConnectionFactory()
        {
            // create broker URI: http://activemq.apache.org/nms/activemq-uri-configuration.html
            var brokerUri = new UriBuilder
            {
                Scheme = UseSsl ? "ssl" : "tcp",
                Host = Host,
                Port = Port
            }.Uri;

            return new ConnectionFactory(brokerUri);
        }

        Uri FormatHostAddress()
        {
            var builder = new UriBuilder
            {
                Scheme = "activemq",
                Host = Host,
                Port = Port,
                Path = "/"
            };

            return builder.Uri;
        }

        public override string ToString()
        {
            return _hostAddress.Value.ToString();
        }
    }
}