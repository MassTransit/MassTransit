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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using Builders;
    using Configurators;
    using EndpointConfigurators;
    using PipeConfigurators;
    using Pipeline;
    using Pipeline.Filters;
    using Pipeline.Pipes;
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
            builder.AddReceiveEndpoint(CreateReceiveEndpoint(builder));
        }

        public void AddPipeBuilderConfigurator(IPipeBuilderConfigurator<ConsumeContext> configurator)
        {
            ((IPipeConfigurator<ConsumeContext>)_pipeConfigurator).AddPipeBuilderConfigurator(configurator);
        }

        public void AddConfigurator(IReceiveEndpointBuilderConfigurator configurator)
        {
            _configurators.Add(configurator);
        }

        ReceiveEndpoint CreateReceiveEndpoint(IInMemoryServiceBusBuilder builder)
        {
            IReceiveTransport transport = builder.ReceiveTransportProvider.GetReceiveTransport(_queueName);

            ConsumePipe consumePipe = CreateInboundPipe();

            IPipe<ReceiveContext> receivePipe = CreateReceivePipe(builder, consumePipe);

            return new ReceiveEndpoint(transport, receivePipe, consumePipe);
        }

        IPipe<ReceiveContext> CreateReceivePipe(IInMemoryServiceBusBuilder builder, ConsumePipe consumePipe)
        {
            IReceiveEndpointBuilder endpointBuilder = new ReceiveEndpointBuilder(consumePipe);

            foreach (IReceiveEndpointBuilderConfigurator builderConfigurator in _configurators)
                builderConfigurator.Configure(endpointBuilder);

            // TODO insert filter that if other excpetion 'n' times move to error
            // TODO insert dead-letter filter so that no message consumers moves to "_skipped"

            var errorAddress = new Uri(string.Format("loopback://localhost/{0}_error", _queueName));
            ISendTransport errorTransport = builder.SendTransportProvider.GetSendTransport(errorAddress);

            IPipe<ReceiveContext> moveToErrorPipe =
                Pipe.New<ReceiveContext>(x => x.Filter(new MoveToErrorTransportFilter(Task.FromResult(errorTransport))));

            _receivePipeConfigurator.Rescue(moveToErrorPipe, typeof(SerializationException));

            _receivePipeConfigurator.Filter(new DeserializeFilter(builder.MessageDeserializer, consumePipe));

            return _receivePipeConfigurator.Build();
        }

        ConsumePipe CreateInboundPipe()
        {
            return new ConsumePipe(_pipeConfigurator);
        }
    }
}