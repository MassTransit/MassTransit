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
namespace MassTransit.EndpointConfigurators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Builders;
    using BusConfigurators;
    using Configurators;
    using PipeConfigurators;
    using Pipeline;
    using Pipeline.Filters;
    using Transports;


    public abstract class ReceiveEndpointConfigurator :
        IConsumePipeConfigurator
    {
        readonly IConsumePipe _consumePipe;
        readonly ConsumePipeSpecificationList _consumePipeSpecification;
        readonly IBuildPipeConfigurator<ReceiveContext> _receiveConfigurator;
        readonly IList<IReceiveEndpointSpecification> _specifications;

        protected ReceiveEndpointConfigurator(IConsumePipe consumePipe)
        {
            _consumePipe = consumePipe;

            _specifications = new List<IReceiveEndpointSpecification>();
            _consumePipeSpecification = new ConsumePipeSpecificationList();
            _receiveConfigurator = new PipeConfigurator<ReceiveContext>();
        }

        void IPipeConfigurator<ConsumeContext>.AddPipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            _consumePipeSpecification.Add(specification);
        }

        void IConsumePipeConfigurator.AddPipeSpecification<T>(IPipeSpecification<ConsumeContext<T>> specification)
        {
            _consumePipeSpecification.Add(specification);
        }

        public virtual IEnumerable<ValidationResult> Validate()
        {
            return _specifications.SelectMany(x => x.Validate());
        }

        public void AddEndpointSpecification(IReceiveEndpointSpecification configurator)
        {
            _specifications.Add(configurator);
        }

        protected IPipe<ReceiveContext> CreateReceivePipe(IBusBuilder builder, Func<IConsumePipe, IReceiveEndpointBuilder> endpointBuilderFactory)
        {
            IConsumePipe consumePipe = _consumePipe ?? builder.CreateConsumePipe(_consumePipeSpecification);

            IReceiveEndpointBuilder endpointBuilder = endpointBuilderFactory(consumePipe);

            foreach (IReceiveEndpointSpecification specification in _specifications)
                specification.Configure(endpointBuilder);

            // TODO insert dead-letter filter so that no message consumers moves to "_skipped"

            ConfigureRescueFilter(builder);

            _receiveConfigurator.UseFilter(new DeserializeFilter(builder.MessageDeserializer, consumePipe));

            return _receiveConfigurator.Build();
        }

        void ConfigureRescueFilter(IBusBuilder builder)
        {
            ISendTransportProvider transportProvider = builder.SendTransportProvider;

            IPipe<ReceiveContext> moveToErrorPipe = Pipe.New<ReceiveContext>(x =>
            {
                Uri errorAddress = GetErrorAddress();

                Func<Task<ISendTransport>> getErrorTransport = () => transportProvider.GetSendTransport(errorAddress);

                x.UseFilter(new MoveToErrorTransportFilter(getErrorTransport));
            });

            _receiveConfigurator.UseRescue(moveToErrorPipe);
        }

        protected abstract Uri GetErrorAddress();
    }
}