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
namespace MassTransit.HttpTransport.Configuration.Builders
{
    using System;
    using BusConfigurators;
    using Clients;
    using Hosting;
    using MassTransit.Builders;
    using MassTransit.Pipeline;
    using Transports;


    public class HttpBusBuilder :
        BusBuilder
    {
        readonly HttpReceiveEndpointConfigurator _busEndpointConfigurator;
        readonly HttpHost[] _hosts;

        public HttpBusBuilder(HttpHost[] hosts,
            IConsumePipeFactory consumePipeFactory,
            ISendPipeFactory sendPipeFactory,
            IPublishPipeFactory publishPipeFactory,
            ReceiveSettings receiveSettings)
            : base(consumePipeFactory, sendPipeFactory, publishPipeFactory, hosts)
        {
            _hosts = hosts;
            _busEndpointConfigurator = new HttpReceiveEndpointConfigurator(_hosts[0], receiveSettings, ConsumePipe);
        }

        protected override Uri GetInputAddress()
        {
            return _busEndpointConfigurator.InputAddress;
        }

        protected override IConsumePipe GetConsumePipe()
        {
            return CreateConsumePipe();
        }

        protected override ISendTransportProvider CreateSendTransportProvider()
        {
            return new HttpSendTransportProvider(_hosts);
        }

        public override ISendEndpointProvider CreateSendEndpointProvider(params ISendPipeSpecification[] specifications)
        {
            var pipe = CreateSendPipe(specifications);

            var provider = new HttpSendEndpointProvider(MessageSerializer, InputAddress, SendTransportProvider, pipe);

            return new SendEndpointCache(provider, CacheDurationProvider);
        }

        TimeSpan CacheDurationProvider(Uri address)
        {
            return TimeSpan.FromHours(1);
        }

        public override IPublishEndpointProvider CreatePublishEndpointProvider(params IPublishPipeSpecification[] specifications)
        {
            return new HttpPublishEndpointProvider();
        }
    }
}