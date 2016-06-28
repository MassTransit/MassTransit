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
namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Integration;
    using Topology;
    using Transports;


    public class RabbitMqSendTransportProvider :
        ISendTransportProvider
    {
        readonly IRabbitMqHost[] _hosts;
        readonly ModelSettings _settings;

        public RabbitMqSendTransportProvider(IRabbitMqHost[] hosts, ModelSettings settings)
        {
            _hosts = hosts;
            _settings = settings;
        }

        public Task<ISendTransport> GetSendTransport(Uri address)
        {
            var hostSettings = address.GetHostSettings();

            var host = _hosts.FirstOrDefault(x => RabbitMqHostEqualityComparer.Default.Equals(hostSettings, x.Settings));
            if (host == null)
                throw new EndpointNotFoundException("The endpoint address specified an unknown host: " + address);

            var sendSettings = address.GetSendSettings(host.Settings.ExchangeTypeDeterminer, host.Settings.RoutingkeyFormatter);

            var modelCache = new RabbitMqModelCache(host.ConnectionCache, host.Supervisor, _settings);

            return Task.FromResult<ISendTransport>(new RabbitMqSendTransport(modelCache, sendSettings));
        }
    }
}