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
    using Apache.NMS.ActiveMQ.Transport;
    using Apache.NMS.ActiveMQ.Transport.Tcp;
    using Apache.NMS.ActiveMQ.Util;


    public class ConfigurationHostSettings :
        ActiveMqHostSettings
    {
        readonly Lazy<Uri> _hostAddress;
        readonly Lazy<Uri> _brokerAddress;

        public ConfigurationHostSettings()
        {
            _hostAddress = new Lazy<Uri>(FormatHostAddress);
            _brokerAddress = new Lazy<Uri>(FormatBrokerAddress);
        }

        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool UseSsl { get; set; }

        public Uri HostAddress => _hostAddress.Value;
        public Uri BrokerAddress => _brokerAddress.Value;

        public IConnection CreateConnection()
        {
            ITransport transport;
            if (UseSsl)
            {
                var sslTransportFactory = new SslTransportFactory
                {
                    SslProtocol = "Tls"
                };

                transport = sslTransportFactory.CreateTransport(BrokerAddress);
            }
            else
            {
                transport = TransportFactory.CreateTransport(BrokerAddress);
            }

            return new Connection(BrokerAddress, transport, new IdGenerator())
            {
                UserName = Username,
                Password = Password
            };
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

        Uri FormatBrokerAddress()
        {
            // create broker URI: http://activemq.apache.org/nms/activemq-uri-configuration.html
            var builder = new UriBuilder
            {
                Scheme = UseSsl ? "ssl" : "tcp",
                Host = Host,
                Port = Port,
                Query = "wireFormat.tightEncodingEnabled=true&nms.AsyncSend=true"
            };

            return builder.Uri;
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