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
namespace MassTransit.PublishPipeSpecifications
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Context;
    using GreenPipes;
    using GreenPipes.Pipes;
    using GreenPipes.Specifications;
    using PipeBuilders;
    using Pipeline;
    using Util;


    public class MessagePublishPipeSpecification<TMessage> :
        IMessagePublishPipeSpecification<TMessage>,
        IMessagePublishPipeSpecification
        where TMessage : class
    {
        readonly IList<ISpecificationPipeSpecification<PublishContext<TMessage>>> _implementedMessageTypeSpecifications;
        readonly IList<ISpecificationPipeSpecification<PublishContext<TMessage>>> _parentMessageSpecifications;
        readonly IList<IPipeSpecification<PublishContext<TMessage>>> _specifications;

        public MessagePublishPipeSpecification()
        {
            _specifications = new List<IPipeSpecification<PublishContext<TMessage>>>();
            _implementedMessageTypeSpecifications = new List<ISpecificationPipeSpecification<PublishContext<TMessage>>>();
            _parentMessageSpecifications = new List<ISpecificationPipeSpecification<PublishContext<TMessage>>>();
        }

        public void AddPipeSpecification(IPipeSpecification<PublishContext> specification)
        {
            var splitSpecification = new SplitFilterPipeSpecification<PublishContext<TMessage>, PublishContext>(specification, MergeContext, FilterContext);

            _specifications.Add(splitSpecification);
        }

        IMessagePublishPipeSpecification<T> IMessagePublishPipeSpecification.GetMessageSpecification<T>()
        {
            var result = this as IMessagePublishPipeSpecification<T>;
            if (result == null)
                throw new ArgumentException($"The expected message type was invalid: {TypeMetadataCache<T>.ShortName}");

            return result;
        }

        public ConnectHandle Connect(IPipeConnector connector)
        {
            IPipe<PublishContext<TMessage>> messagePipe = BuildMessagePipe();

            if (messagePipe is EmptyPipe<PublishContext<TMessage>>)
                return new EmptyConnectHandle();

            return connector.ConnectPipe(messagePipe);
        }

        public void AddPipeSpecification(IPipeSpecification<PublishContext<TMessage>> specification)
        {
            _specifications.Add(specification);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _specifications.SelectMany(x => x.Validate());
        }

        public void Apply(ISpecificationPipeBuilder<PublishContext<TMessage>> builder)
        {
            if (!builder.IsDelegated)
            {
                ISpecificationPipeBuilder<PublishContext<TMessage>> implementedBuilder = builder.CreateImplementedBuilder();

                foreach (ISpecificationPipeSpecification<PublishContext<TMessage>> specification in _implementedMessageTypeSpecifications.Reverse())
                {
                    specification.Apply(implementedBuilder);
                }
            }

            ISpecificationPipeBuilder<PublishContext<TMessage>> delegatedBuilder = builder.CreateDelegatedBuilder();

            foreach (ISpecificationPipeSpecification<PublishContext<TMessage>> specification in _parentMessageSpecifications)
            {
                specification.Apply(delegatedBuilder);
            }

            foreach (IPipeSpecification<PublishContext<TMessage>> specification in _specifications)
            {
                specification.Apply(builder);
            }
        }

        public IPipe<PublishContext<TMessage>> BuildMessagePipe()
        {
            var pipeBuilder = new SpecificationPipeBuilder<PublishContext<TMessage>>();

            Apply(pipeBuilder);

            return pipeBuilder.Build();
        }

        public void AddParentMessageSpecification(ISpecificationPipeSpecification<PublishContext<TMessage>> implementedMessageTypeSpecification)
        {
            _parentMessageSpecifications.Add(implementedMessageTypeSpecification);
        }

        public void AddImplementedMessageSpecification<T>(ISpecificationPipeSpecification<PublishContext<T>> implementedMessageTypeSpecification)
            where T : class
        {
            var adapter = new ImplementedTypeAdapter<T>(implementedMessageTypeSpecification);

            _implementedMessageTypeSpecifications.Add(adapter);
        }

        static PublishContext FilterContext(PublishContext<TMessage> context)
        {
            return context;
        }

        static PublishContext<TMessage> MergeContext(PublishContext<TMessage> input, PublishContext context)
        {
            var result = context as PublishContext<TMessage>;

            return result ?? new PublishContextProxy<TMessage>(context, input.Message);
        }


        class ImplementedTypeAdapter<T> :
            ISpecificationPipeSpecification<PublishContext<TMessage>>
            where T : class
        {
            readonly ISpecificationPipeSpecification<PublishContext<T>> _specification;

            public ImplementedTypeAdapter(ISpecificationPipeSpecification<PublishContext<T>> specification)
            {
                _specification = specification;
            }

            public void Apply(ISpecificationPipeBuilder<PublishContext<TMessage>> builder)
            {
                var specification = new MessagePublishPipeSplitFilterSpecification<TMessage, T>(_specification);

                specification.Apply(builder);
            }

            public IEnumerable<ValidationResult> Validate()
            {
                yield break;
            }
        }
    }
}