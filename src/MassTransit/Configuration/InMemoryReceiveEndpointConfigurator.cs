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
namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using Builders;
    using Configurators;
    using Logging;
    using PipeConfigurators;
    using Pipeline;
    using Pipeline.Filters;
    using Transports;


    public class InMemoryReceiveEndpointConfigurator :
        IReceiveEndpointConfigurator,
        IInMemoryBusFactorySpecification
    {
        static readonly ILog _log = Logger.Get<InMemoryReceiveEndpointConfigurator>();

        readonly IList<IReceiveEndpointSpecification> _configurators;
        readonly IConsumePipe _consumePipe;
        readonly IList<IPipeSpecification<ConsumeContext>> _consumePipeSpecifications;
        readonly string _queueName;
        readonly IBuildPipeConfigurator<ReceiveContext> _receivePipeConfigurator;

        public InMemoryReceiveEndpointConfigurator(string queueName, IConsumePipe consumePipe = null)
        {
            _queueName = queueName;
            _consumePipe = consumePipe;
            _receivePipeConfigurator = new PipeConfigurator<ReceiveContext>();
            _configurators = new List<IReceiveEndpointSpecification>();

            _consumePipeSpecifications = new List<IPipeSpecification<ConsumeContext>>();
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _configurators.SelectMany(x => x.Validate());
        }

        public void Apply(IInMemoryBusBuilder builder)
        {
            builder.AddReceiveEndpoint(CreateReceiveEndpoint(builder));
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            _consumePipeSpecifications.Add(specification);
        }

        public void AddEndpointSpecification(IReceiveEndpointSpecification configurator)
        {
            _configurators.Add(configurator);
        }

        ReceiveEndpoint CreateReceiveEndpoint(IInMemoryBusBuilder builder)
        {
            IReceiveTransport transport = builder.ReceiveTransportProvider.GetReceiveTransport(_queueName);

            IConsumePipe consumePipe = _consumePipe ?? builder.CreateConsumePipe(_consumePipeSpecifications);

            if (_log.IsDebugEnabled)
            {
                var inspector = new StringPipelineVisitor();
                consumePipe.Visit(inspector);

                _log.Debug(inspector.ToString());
            }

            IPipe<ReceiveContext> receivePipe = CreateReceivePipe(builder, consumePipe);

            return new ReceiveEndpoint(transport, receivePipe);
        }

        IPipe<ReceiveContext> CreateReceivePipe(IInMemoryBusBuilder builder, IConsumePipe consumePipe)
        {
            IReceiveEndpointBuilder endpointBuilder = new InMemoryReceiveEndpointBuilder(consumePipe);

            foreach (IReceiveEndpointSpecification builderConfigurator in _configurators)
                builderConfigurator.Configure(endpointBuilder);

            // TODO insert filter that if other excpetion 'n' times move to error
            // TODO insert dead-letter filter so that no message consumers moves to "_skipped"

            var errorAddress = new Uri(string.Format("loopback://localhost/{0}_error", _queueName));
            ISendTransportProvider sendTransportProvider = builder.SendTransportProvider;

            IPipe<ReceiveContext> moveToErrorPipe = Pipe.New<ReceiveContext>(
                x => x.Filter(new MoveToErrorTransportFilter(() => Task.FromResult(sendTransportProvider.GetSendTransport(errorAddress)))));

            _receivePipeConfigurator.Rescue(moveToErrorPipe, typeof(SerializationException));

            _receivePipeConfigurator.Filter(new DeserializeFilter(builder.MessageDeserializer, consumePipe));

            return _receivePipeConfigurator.Build();
        }
    }
}