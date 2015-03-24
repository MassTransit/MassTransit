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
    using System.Text;
    using MassTransit.Builders;
    using MassTransit.Pipeline;
    using PipeConfigurators;
    using Transports;
    using Util;


    public class RabbitMqBusBuilder :
        BusBuilder,
        IBusBuilder
    {
        readonly IConsumePipe _busConsumePipe;
        readonly RabbitMqReceiveEndpointConfigurator _busEndpointConfigurator;
        readonly IRabbitMqHost[] _hosts;

        public RabbitMqBusBuilder(IEnumerable<IRabbitMqHost> hosts, IEnumerable<IPipeSpecification<ConsumeContext>> endpointPipeSpecifications)
            : base(endpointPipeSpecifications)
        {
            _hosts = hosts.ToArray();

            string machineName = GetSanitizedQueueNameString(HostMetadataCache.Host.MachineName);
            string processName = GetSanitizedQueueNameString(HostMetadataCache.Host.ProcessName);
            string queueName = string.Format("bus-{0}-{1}-{2}", processName, machineName, NewId.Next().ToString("NS"));

            _busConsumePipe = CreateConsumePipe(Enumerable.Empty<IPipeSpecification<ConsumeContext>>());

            _busEndpointConfigurator = new RabbitMqReceiveEndpointConfigurator(_hosts[0], queueName, _busConsumePipe);
            _busEndpointConfigurator.Exclusive();
            _busEndpointConfigurator.Durable(false);
            _busEndpointConfigurator.AutoDelete();
        }

        public IBusControl Build()
        {
            _busEndpointConfigurator.Apply(this);

            return new MassTransitBus(_busEndpointConfigurator.InputAddress, _busConsumePipe, SendEndpointProvider, PublishEndpoint, ReceiveEndpoints, _hosts);
        }

        protected override Uri GetInputAddress()
        {
            return _busEndpointConfigurator.InputAddress;
        }

        protected override ISendTransportProvider CreateSendTransportProvider()
        {
            return new RabbitMqSendTransportProvider(_hosts);
        }

        protected override ISendEndpointProvider CreateSendEndpointProvider()
        {
            var sendEndpointProvider = new RabbitMqSendEndpointProvider(MessageSerializer, InputAddress, SendTransportProvider);

            return new SendEndpointCache(sendEndpointProvider);
        }

        protected override IPublishEndpoint CreatePublishEndpoint()
        {
            return new RabbitMqPublishEndpoint(_hosts[0], MessageSerializer, InputAddress);
        }

        string GetSanitizedQueueNameString(string input)
        {
            var sb = new StringBuilder(input.Length);

            foreach (char c in input)
            {
                if (char.IsLetterOrDigit(c))
                    sb.Append(c);
                else if (c == '.' || c == '_' || c == '-' || c == ':')
                    sb.Append(c);
            }

            return sb.ToString();
        }
    }
}