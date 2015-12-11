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
    using Builders;
    using BusConfigurators;
    using MassTransit.Pipeline;
    using Transports;


    public class ServiceBusBusBuilder :
        BusBuilder
    {
        readonly ServiceBusReceiveEndpointConfigurator _busEndpointConfigurator;
        readonly ServiceBusHost[] _hosts;

        public ServiceBusBusBuilder(ServiceBusHost[] hosts, IConsumePipeFactory consumePipeFactory, ISendPipeFactory sendPipeFactory,
            IPublishPipeFactory publishPipeFactory, ReceiveEndpointSettings settings)
            : base(consumePipeFactory, sendPipeFactory, publishPipeFactory, hosts)
        {
            if (hosts == null)
                throw new ArgumentNullException(nameof(hosts));

            _hosts = hosts;

            _busEndpointConfigurator = new ServiceBusReceiveEndpointConfigurator(_hosts[0], settings, ConsumePipe);
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
            return new ServiceBusSendTransportProvider(_hosts);
        }

        public override ISendEndpointProvider CreateSendEndpointProvider(params ISendPipeSpecification[] specifications)
        {
            var pipe = CreateSendPipe(specifications);

            var provider = new ServiceBusSendEndpointProvider(MessageSerializer, InputAddress, SendTransportProvider, pipe);

            return new SendEndpointCache(provider);
        }

        public override IPublishEndpointProvider CreatePublishEndpointProvider(params IPublishPipeSpecification[] specifications)
        {
            var provider = new PublishSendEndpointProvider(MessageSerializer, InputAddress, _hosts);

            var cache = new SendEndpointCache(provider);

            var pipe = CreatePublishPipe(specifications);

            return new ServiceBusPublishEndpointProvider(_hosts[0], cache, pipe);
        }

        protected override void PreBuild()
        {
            base.PreBuild();

            _busEndpointConfigurator.Apply(this);
        }
    }
}