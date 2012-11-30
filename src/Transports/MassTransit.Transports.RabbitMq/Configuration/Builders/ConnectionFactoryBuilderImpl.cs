// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Transports.RabbitMq.Configuration.Builders
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Magnum.Extensions;
    using RabbitMQ.Client;

    public class ConnectionFactoryBuilderImpl :
        ConnectionFactoryBuilder
    {
        readonly IRabbitMqEndpointAddress _address;
        readonly IList<Func<ConnectionFactory, ConnectionFactory>> _connectionFactoryConfigurators;

        public ConnectionFactoryBuilderImpl(IRabbitMqEndpointAddress address)
        {
            _address = address;
            _connectionFactoryConfigurators = new List<Func<ConnectionFactory, ConnectionFactory>>();
        }

        public ConnectionFactory Build()
        {
            ConnectionFactory connectionFactory = _address.ConnectionFactory;

            var clientProperties = new Dictionary<string, string>();


            clientProperties.Add("client_api", "MassTransit");
            clientProperties.Add("masstransit_version", typeof(IServiceBus).Assembly.GetName().Version.ToString());
            clientProperties.Add("net_version", Environment.Version.ToString());
            clientProperties.Add("hostname", Environment.MachineName);
            clientProperties.Add("connected", DateTimeOffset.Now.ToString("R"));
            clientProperties.Add("process_id", Process.GetCurrentProcess().Id.ToString());
            clientProperties.Add("process_name", Process.GetCurrentProcess().ProcessName);


            connectionFactory.ClientProperties = clientProperties;

            _connectionFactoryConfigurators.Each(x => x(connectionFactory));

            return connectionFactory;
        }

        public void Add(Func<ConnectionFactory, ConnectionFactory> callback)
        {
            _connectionFactoryConfigurators.Add(callback);
        }
    }
}