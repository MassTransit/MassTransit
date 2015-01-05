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
namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Builders;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Pipes;
    using Transports;


    public class AzureServiceBusBusBuilder :
        BusBuilder,
        IBusBuilder
    {
        readonly ServiceBusHostSettings[] _hosts;
        readonly Uri _sourceAddress;

        public AzureServiceBusBusBuilder(IEnumerable<ServiceBusHostSettings> hosts, Uri sourceAddress)
        {
            if (hosts == null)
                throw new ArgumentNullException("hosts");

            _hosts = hosts.ToArray();

            _sourceAddress = sourceAddress;
        }

        protected override ISendEndpointProvider CreateSendEndpointProvider()
        {
            var provider = new AzureServiceBusSendEndpointProvider(MessageSerializer, _sourceAddress, _hosts);

            return new SendEndpointCache(provider);
        }

        protected override IPublishEndpoint CreatePublishEndpoint()
        {
            return new AzureServiceBusPublishEndpoint(SendEndpointProvider);
        }

        public virtual IBusControl Build()
        {
            IConsumePipe consumePipe = ReceiveEndpoints.Where(x => x.InputAddress.Equals(_sourceAddress))
                .Select(x => x.ConsumePipe).FirstOrDefault() ?? new ConsumePipe();

            var endpointCache = new SendEndpointCache(SendEndpointProvider);

            return new MassTransitBus(_sourceAddress, consumePipe, endpointCache, PublishEndpoint, ReceiveEndpoints);
        }
    }
}