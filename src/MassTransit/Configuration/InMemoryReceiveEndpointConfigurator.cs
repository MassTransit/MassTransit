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
namespace MassTransit
{
    using System.Collections.Generic;
    using System.Linq;
    using Builders;
    using Configurators;
    using EndpointConfigurators;
    using PipeConfigurators;
    using Pipeline;
    using Transports;


    public class InMemoryReceiveEndpointConfigurator :
        IReceiveEndpointConfigurator,
        IInMemoryServiceBusFactoryBuilderConfigurator
    {
        readonly IList<IReceiveEndpointBuilderConfigurator> _configurators;
        readonly PipeConfigurator<ConsumeContext> _pipeConfigurator;
        readonly string _queueName;
        readonly PipeConfigurator<ReceiveContext> _receivePipeConfigurator;

        public InMemoryReceiveEndpointConfigurator(string queueName)
        {
            _queueName = queueName;
            _pipeConfigurator = new PipeConfigurator<ConsumeContext>();
            _receivePipeConfigurator = new PipeConfigurator<ReceiveContext>();
            _configurators = new List<IReceiveEndpointBuilderConfigurator>();
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _configurators.SelectMany(x => x.Validate());
        }

        public void Configure(IInMemoryServiceBusBuilder builder)
        {
            builder.AddReceiveEndpoint(CreateReceiveEndpoint(builder.MessageDeserializer, builder.ReceiveTransportProvider));
        }

        public void AddPipeBuilderConfigurator(IPipeBuilderConfigurator<ConsumeContext> configurator)
        {
            _pipeConfigurator.AddPipeBuilderConfigurator(configurator);
        }

        public void AddConfigurator(IReceiveEndpointBuilderConfigurator configurator)
        {
            _configurators.Add(configurator);
        }

        public ReceiveEndpoint CreateReceiveEndpoint(IMessageDeserializer deserializer, IReceiveTransportProvider receiveTransportProvider)
        {
            IReceiveTransport transport = receiveTransportProvider.GetReceiveTransport(_queueName);

            InboundPipe inboundPipe = CreateInboundPipe();

            IPipe<ReceiveContext> receivePipe = CreateReceivePipe(deserializer, inboundPipe);

            return new ReceiveEndpoint(transport, receivePipe, inboundPipe);
        }

        IPipe<ReceiveContext> CreateReceivePipe(IMessageDeserializer deserializer, InboundPipe inboundPipe)
        {
            IReceiveEndpointBuilder builder = new ReceiveEndpointBuilder(inboundPipe);

            foreach (IReceiveEndpointBuilderConfigurator builderConfigurator in _configurators)
                builderConfigurator.Configure(builder);

            _receivePipeConfigurator.Filter(new DeserializeFilter(deserializer, inboundPipe));

            return _receivePipeConfigurator.Build();
        }

        InboundPipe CreateInboundPipe()
        {
            return new InboundPipe(_pipeConfigurator);
        }
    }
}