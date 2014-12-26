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
namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Builders;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Pipes;
    using Transports;


    public class AzureServiceBusServiceBusBuilder :
        ServiceBusBuilderBase,
        IServiceBusBuilder
    {
        readonly ServiceBusHostSettings[] _hosts;
        readonly Uri _sourceAddress;

        public AzureServiceBusServiceBusBuilder(IEnumerable<ServiceBusHostSettings> hosts)
        {
            if (hosts == null)
                throw new ArgumentNullException("hosts");

            _hosts = hosts.ToArray();

            _sourceAddress = new Uri(_hosts[0].ServiceUri, NewId.Next().ToString("NS"));
        }

        protected override ISendEndpointProvider CreateSendEndpointProvider()
        {
            return new AzureServiceBusSendEndpointProvider(MessageSerializer, _sourceAddress, _hosts);
        }

        protected override IPublishEndpoint CreatePublishEndpoint()
        {
            return new AzureServiceBusPublishEndpoint(SendEndpointProvider);
        }

        public virtual IBusControl Build()
        {
            IConsumePipe consumePipe = new ConsumePipe();

            var endpointCache = new SendEndpointCache(SendEndpointProvider);

            return new MassTransitBus(_sourceAddress, consumePipe, endpointCache, PublishEndpoint, ReceiveEndpoints);
        }
    }
}