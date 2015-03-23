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
    using System.Collections.Generic;
    using System.Linq;
    using MassTransit.Builders;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Pipes;
    using PipeConfigurators;
    using Transports;


    public class RabbitMqBusBuilder :
        BusBuilder,
        IBusBuilder
    {
        readonly IRabbitMqHost[] _hosts;
        readonly Uri _sourceAddress;

        public RabbitMqBusBuilder(IEnumerable<IRabbitMqHost> hosts, Uri sourceAddress, IEnumerable<IPipeSpecification<ConsumeContext>> endpointPipeSpecifications)
            : base(endpointPipeSpecifications)
        {
            _hosts = hosts.ToArray();
            _sourceAddress = sourceAddress;
        }

        public IBusControl Build()
        {
            IConsumePipe consumePipe = ReceiveEndpoints.Where(x => x.InputAddress.Equals(_sourceAddress))
                .Select(x => x.ConsumePipe).FirstOrDefault() ?? new ConsumePipe();

            ISendEndpointProvider sendEndpointProvider = SendEndpointProvider;
            return new MassTransitBus(_sourceAddress, consumePipe, sendEndpointProvider, PublishEndpoint, ReceiveEndpoints, _hosts);
        }

        protected override ISendEndpointProvider CreateSendEndpointProvider()
        {
            var sendEndpointProvider = new RabbitMqSendEndpointProvider(MessageSerializer, _hosts, _sourceAddress);

            return new SendEndpointCache(sendEndpointProvider);
        }

        protected override IPublishEndpoint CreatePublishEndpoint()
        {
            return new RabbitMqPublishEndpoint(_hosts[0], MessageSerializer, _sourceAddress);
        }
    }
}