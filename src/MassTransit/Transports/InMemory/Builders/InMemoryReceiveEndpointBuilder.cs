// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Transports.InMemory.Builders
{
    using System.Linq;
    using Configuration;
    using Context;
    using Contexts;
    using GreenPipes;
    using MassTransit.Builders;


    public class InMemoryReceiveEndpointBuilder :
        ReceiveEndpointBuilder,
        IReceiveEndpointBuilder
    {
        readonly InMemoryHost _host;
        readonly IInMemoryReceiveEndpointConfiguration _configuration;

        public InMemoryReceiveEndpointBuilder(InMemoryHost host, IInMemoryReceiveEndpointConfiguration configuration)
            : base(configuration)
        {
            _host = host;
            _configuration = configuration;
        }

        public override ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
        {
            _configuration.Topology.Consume
                .GetMessageTopology<T>()
                .Bind();

            return base.ConnectConsumePipe(pipe);
        }

        public ReceiveEndpointContext CreateReceiveEndpointContext()
        {
            var builder = _host.CreateConsumeTopologyBuilder();

            var queueName = _configuration.InputAddress.AbsolutePath.Split('/').Last();

            builder.Queue = queueName;
            builder.QueueDeclare(queueName);
            builder.Exchange = queueName;
            builder.QueueBind(builder.Exchange, builder.Queue);

            _configuration.Topology.Consume.Apply(builder);

            return new InMemoryReceiveEndpointContext(_configuration, _host, ReceiveObservers, TransportObservers, EndpointObservers);
        }
    }
}