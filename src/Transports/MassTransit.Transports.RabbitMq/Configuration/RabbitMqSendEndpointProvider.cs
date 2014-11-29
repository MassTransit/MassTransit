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
namespace MassTransit.Transports.RabbitMq.Configuration
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Policies;
    using RabbitMQ.Client;


    public class RabbitMqSendEndpointProvider :
        ISendEndpointProvider
    {
        readonly RabbitMqHostSettings[] _hosts;
        readonly ISendMessageSerializer _serializer;
        readonly Uri _inputAddress;

        public RabbitMqSendEndpointProvider(ISendMessageSerializer serializer, RabbitMqHostSettings[] hosts, Uri inputAddress)
        {
            _hosts = hosts;
            _inputAddress = inputAddress;
            _serializer = serializer;
        }

        public async Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            RabbitMqHostSettings host = _hosts
                .Where(x => x.Host.Equals(address.Host, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();
            if (host == null)
                throw new EndpointNotFoundException("The endpoint address specified an unknown host: " + address);


            RabbitMqHostSettings hostSettings = address.GetHostSettings();


            SendSettings sendSettings = address.GetSendSettings();

            ConnectionFactory connectionFactory = hostSettings.GetConnectionFactory();

            var connectionCache = new RabbitMqConnectionCache(new RabbitMqConnector(connectionFactory, Retry.None));

            var modelCache = new RabbitMqModelCache(connectionCache);

            var sendTransport = new RabbitMqSendTransport(modelCache, sendSettings);


            return new SendEndpoint(sendTransport, _serializer, address, _inputAddress);
        }
    }
}