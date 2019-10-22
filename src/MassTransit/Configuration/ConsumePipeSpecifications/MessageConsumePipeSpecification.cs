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
namespace MassTransit.ConsumePipeSpecifications
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Context;
    using GreenPipes;
    using GreenPipes.Pipes;
    using GreenPipes.Specifications;
    using GreenPipes.Util;
    using Metadata;
    using PipeBuilders;
    using Util;


    public class MessageConsumePipeSpecification<TMessage> :
        IMessageConsumePipeSpecification<TMessage>,
        IMessageConsumePipeSpecification
        where TMessage : class
    {
        readonly IList<ISpecificationPipeSpecification<ConsumeContext<TMessage>>> _implementedMessageTypeSpecifications;
        readonly IList<ISpecificationPipeSpecification<ConsumeContext<TMessage>>> _parentMessageSpecifications;
        readonly IList<IPipeSpecification<ConsumeContext<TMessage>>> _specifications;
        readonly IList<IPipeSpecification<ConsumeContext>> _baseSpecifications;

        public MessageConsumePipeSpecification()
        {
            _specifications = new List<IPipeSpecification<ConsumeContext<TMessage>>>();
            _baseSpecifications = new List<IPipeSpecification<ConsumeContext>>();
            _implementedMessageTypeSpecifications = new List<ISpecificationPipeSpecification<ConsumeContext<TMessage>>>();
            _parentMessageSpecifications = new List<ISpecificationPipeSpecification<ConsumeContext<TMessage>>>();
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            _baseSpecifications.Add(specification);
        }

        IMessageConsumePipeSpecification<T> IMessageConsumePipeSpecification.GetMessageSpecification<T>()
        {
            var result = this as IMessageConsumePipeSpecification<T>;
            if (result == null)
                throw new ArgumentException($"The expected message type was invalid: {TypeMetadataCache<T>.ShortName}");

            return result;
        }

        public ConnectHandle Connect(IPipeConnector connector)
        {
            IPipe<ConsumeContext<TMessage>> messagePipe = BuildMessagePipe();

            if (messagePipe is EmptyPipe<ConsumeContext<TMessage>>)
                return new EmptyConnectHandle();

            return connector.ConnectPipe(messagePipe);
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumeContext<TMessage>> specification)
        {
            _specifications.Add(specification);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _specifications.SelectMany(x => x.Validate());
        }

        public void Apply(ISpecificationPipeBuilder<ConsumeContext<TMessage>> builder)
        {
            if (!builder.IsDelegated && _parentMessageSpecifications.Count > 0)
            {
                ISpecificationPipeBuilder<ConsumeContext<TMessage>> delegatedBuilder = builder.CreateDelegatedBuilder();

                foreach (ISpecificationPipeSpecification<ConsumeContext<TMessage>> specification in _parentMessageSpecifications)
                {
                    specification.Apply(delegatedBuilder);
                }
            }

            foreach (IPipeSpecification<ConsumeContext<TMessage>> specification in _specifications)
            {
                specification.Apply(builder);
            }

            if (!builder.IsImplemented)
            {
                foreach (IPipeSpecification<ConsumeContext> specification in _baseSpecifications)
                {
                    var split = new SplitFilterPipeSpecification<ConsumeContext<TMessage>, ConsumeContext>(specification, MergeContext, FilterContext);
                    split.Apply(builder);
                }
            }
        }

        public IPipe<ConsumeContext<TMessage>> BuildMessagePipe()
        {
            var pipeBuilder = new SpecificationPipeBuilder<ConsumeContext<TMessage>>();

            Apply(pipeBuilder);

            return pipeBuilder.Build();
        }

        public IPipe<ConsumeContext<TMessage>> BuildMessagePipe(IPipe<ConsumeContext<TMessage>> pipe)
        {
            var pipeBuilder = new SpecificationPipeBuilder<ConsumeContext<TMessage>>();

            Apply(pipeBuilder);

            return pipeBuilder.Build(pipe);
        }

        public void AddParentMessageSpecification(ISpecificationPipeSpecification<ConsumeContext<TMessage>> parentSpecification)
        {
            _parentMessageSpecifications.Add(parentSpecification);
        }

        public void AddImplementedMessageSpecification<T>(ISpecificationPipeSpecification<ConsumeContext<T>> implementedSpecification)
            where T : class
        {
            var adapter = new ImplementedTypeAdapter<T>(implementedSpecification);

            _implementedMessageTypeSpecifications.Add(adapter);
        }

        static ConsumeContext FilterContext(ConsumeContext<TMessage> context)
        {
            return context;
        }

        static ConsumeContext<TMessage> MergeContext(ConsumeContext<TMessage> input, ConsumeContext context)
        {
            var result = context as ConsumeContext<TMessage>;

            return result ?? new MessageConsumeContext<TMessage>(context, input.Message);
        }


        class ImplementedTypeAdapter<T> :
            ISpecificationPipeSpecification<ConsumeContext<TMessage>>
            where T : class
        {
            readonly ISpecificationPipeSpecification<ConsumeContext<T>> _specification;

            public ImplementedTypeAdapter(ISpecificationPipeSpecification<ConsumeContext<T>> specification)
            {
                _specification = specification;
            }

            public void Apply(ISpecificationPipeBuilder<ConsumeContext<TMessage>> builder)
            {
                var specification = new MessageConsumePipeSplitFilterSpecification<TMessage, T>(_specification);

                specification.Apply(builder);
            }

            public IEnumerable<ValidationResult> Validate()
            {
                yield break;
            }
        }
    }
}