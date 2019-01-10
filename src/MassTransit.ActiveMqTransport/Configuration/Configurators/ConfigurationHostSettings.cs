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


    public class ConfigurationHostSettings :
        ActiveMqHostSettings
    {
        readonly Lazy<Uri> _hostAddress;
        readonly Lazy<Uri> _brokerAddress;
        readonly Lazy<IConnectionFactory> _connectionFactory;

        public ConfigurationHostSettings()
        {
            _hostAddress = new Lazy<Uri>(FormatHostAddress);
            _brokerAddress = new Lazy<Uri>(FormatBrokerAddress);
            _connectionFactory = new Lazy<IConnectionFactory>(() => new NMSConnectionFactory(BrokerAddress));
        }

        public Uri ConnectionString { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public Uri HostAddress => _hostAddress.Value;
        public Uri BrokerAddress => _brokerAddress.Value;

        public IConnection CreateConnection()
        {
            return _connectionFactory.Value.CreateConnection(Username, Password);
        }

        Uri FormatHostAddress()
        {
            return ConnectionString;
        }

        Uri FormatBrokerAddress()
        {
            return ConnectionString;
        }

        public override string ToString()
        {
            return ConnectionString.ToString();
        }
    }
}