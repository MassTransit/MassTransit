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
namespace MassTransit.RabbitMqTransport.Configuration.Builders
{
    using System;
    using BusConfigurators;
    using MassTransit.Builders;
    using MassTransit.Pipeline;
    using Topology;
    using Transports;


    public class RabbitMqBusBuilder :
        BusBuilder
    {
        readonly RabbitMqReceiveEndpointConfigurator _busEndpointConfigurator;
        readonly RabbitMqHost[] _hosts;

        public RabbitMqBusBuilder(RabbitMqHost[] hosts, IConsumePipeFactory consumePipeFactory, ISendPipeFactory sendPipeFactory,
            RabbitMqReceiveSettings busSettings)
            : base(consumePipeFactory, sendPipeFactory, hosts)
        {
            _hosts = hosts;

            _busEndpointConfigurator = new RabbitMqReceiveEndpointConfigurator(_hosts[0], busSettings, ConsumePipe);
        }

        protected override void PreBuild()
        {
            _busEndpointConfigurator.Apply(this);
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
            return new RabbitMqSendTransportProvider(_hosts);
        }

        public override ISendEndpointProvider CreateSendEndpointProvider(params ISendPipeSpecification[] specifications)
        {
            var sendPipe = CreateSendPipe(specifications);

            var sendEndpointProvider = new RabbitMqSendEndpointProvider(MessageSerializer, InputAddress, SendTransportProvider, sendPipe);

            return new SendEndpointCache(sendEndpointProvider);
        }

        public override IPublishEndpointProvider CreatePublishEndpointProvider()
        {
            var sendPipe = CreateSendPipe();

            return new RabbitMqPublishEndpointProvider(_hosts[0], MessageSerializer, InputAddress, sendPipe);
        }
    }
}