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
namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Configuration;
    using Serialization;
    using Transports;


    public class RabbitMqSendEndpointProvider :
        ISendEndpointProvider,
        IDisposable
    {
        readonly IRabbitMqHost[] _hosts;
        readonly Uri _inputAddress;
        readonly IMessageSerializer _serializer;

        public RabbitMqSendEndpointProvider(IMessageSerializer serializer, IRabbitMqHost[] hosts, Uri inputAddress)
        {
            _hosts = hosts;
            _inputAddress = inputAddress;
            _serializer = serializer;
        }

        public void Dispose()
        {
            foreach (IRabbitMqHost host in _hosts)
            {
                host.ConnectionCache.Stop();
                host.SendConnectionCache.Stop();
            }
        }

        public async Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            SendSettings sendSettings = address.GetSendSettings();

            RabbitMqHostSettings hostSettings = address.GetHostSettings();

            IRabbitMqHost host = _hosts.FirstOrDefault(x => RabbitMqHostEqualityComparer.Default.Equals(hostSettings, x.Settings));
            if (host == null)
                throw new EndpointNotFoundException("The endpoint address specified an unknown host: " + address);

            var modelCache = new RabbitMqModelCache(host.SendConnectionCache);

            var sendTransport = new RabbitMqSendTransport(modelCache, sendSettings);

            return new SendEndpoint(sendTransport, _serializer, address, _inputAddress);
        }
    }
}