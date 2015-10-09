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
    using Pipeline.Pipes;
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
            return _specifications.SelectMany(x => x.Validate())
                .Concat(_consumePipeSpecification.Validate());
        }

        public void AddEndpointSpecification(IReceiveEndpointSpecification configurator)
        {
            _specifications.Add(configurator);
        }

        protected IReceivePipe CreateReceivePipe(IBusBuilder builder, Func<IConsumePipe, IReceiveEndpointBuilder> endpointBuilderFactory)
        {
            IConsumePipe consumePipe = _consumePipe ?? builder.CreateConsumePipe(_consumePipeSpecification);

            IReceiveEndpointBuilder endpointBuilder = endpointBuilderFactory(consumePipe);

            foreach (IReceiveEndpointSpecification specification in _specifications)
                specification.Configure(endpointBuilder);

            ConfigureAddDeadLetterFilter(builder.SendTransportProvider);

            ConfigureRescueFilter(builder.PublishEndpoint, builder.SendTransportProvider);

            _receiveConfigurator.UseFilter(new DeserializeFilter(builder.MessageDeserializer, consumePipe));

            var receivePipe = _receiveConfigurator.Build();

            return new ReceivePipe(receivePipe, consumePipe);
        }

        void ConfigureAddDeadLetterFilter(ISendTransportProvider transportProvider)
        {
            IPipe<ReceiveContext> moveToDeadLetterPipe = Pipe.New<ReceiveContext>(x =>
            {
                Uri deadLetterAddress = GetDeadLetterAddress();

                Func<Task<ISendTransport>> getDeadLetterTransport = () => transportProvider.GetSendTransport(deadLetterAddress);

                x.UseFilter(new MoveToTransportFilter(deadLetterAddress, getDeadLetterTransport, "dead-letter"));
            });

            _receiveConfigurator.UseDeadLetterQueue(moveToDeadLetterPipe);
        }

        void ConfigureRescueFilter(IPublishEndpointProvider publishEndpoint, ISendTransportProvider transportProvider)
        {
            IPipe<ExceptionReceiveContext> moveToErrorPipe = Pipe.New<ExceptionReceiveContext>(x =>
            {
                Uri errorAddress = GetErrorAddress();

                Func<Task<ISendTransport>> getErrorTransport = () => transportProvider.GetSendTransport(errorAddress);

                x.UseFilter(new MoveExceptionToTransportFilter(publishEndpoint, errorAddress, getErrorTransport));
            });

            _receiveConfigurator.UseRescue(moveToErrorPipe);
        }

        protected abstract Uri GetErrorAddress();
        protected abstract Uri GetDeadLetterAddress();
    }
}