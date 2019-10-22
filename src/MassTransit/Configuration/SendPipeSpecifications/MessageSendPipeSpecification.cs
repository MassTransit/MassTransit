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
namespace MassTransit.SendPipeSpecifications
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


    public class MessageSendPipeSpecification<TMessage> :
        IMessageSendPipeSpecification<TMessage>,
        IMessageSendPipeSpecification
        where TMessage : class
    {
        readonly IList<ISpecificationPipeSpecification<SendContext<TMessage>>> _implementedMessageTypeSpecifications;
        readonly IList<ISpecificationPipeSpecification<SendContext<TMessage>>> _parentMessageSpecifications;
        readonly IList<IPipeSpecification<SendContext<TMessage>>> _specifications;
        readonly IList<IPipeSpecification<SendContext>> _baseSpecifications;

        public MessageSendPipeSpecification()
        {
            _specifications = new List<IPipeSpecification<SendContext<TMessage>>>();
            _baseSpecifications = new List<IPipeSpecification<SendContext>>();
            _implementedMessageTypeSpecifications = new List<ISpecificationPipeSpecification<SendContext<TMessage>>>();
            _parentMessageSpecifications = new List<ISpecificationPipeSpecification<SendContext<TMessage>>>();
        }

        public void AddPipeSpecification(IPipeSpecification<SendContext> specification)
        {
            _baseSpecifications.Add(specification);
        }

        IMessageSendPipeSpecification<T> IMessageSendPipeSpecification.GetMessageSpecification<T>()
        {
            var result = this as IMessageSendPipeSpecification<T>;
            if (result == null)
                throw new ArgumentException($"The expected message type was invalid: {TypeMetadataCache<T>.ShortName}");

            return result;
        }

        public ConnectHandle Connect(IPipeConnector connector)
        {
            IPipe<SendContext<TMessage>> messagePipe = BuildMessagePipe();

            if (messagePipe is EmptyPipe<SendContext<TMessage>>)
                return new EmptyConnectHandle();

            return connector.ConnectPipe(messagePipe);
        }

        public void AddPipeSpecification(IPipeSpecification<SendContext<TMessage>> specification)
        {
            _specifications.Add(specification);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _specifications.SelectMany(x => x.Validate());
        }

        public void Apply(ISpecificationPipeBuilder<SendContext<TMessage>> builder)
        {
            if (!builder.IsDelegated && _implementedMessageTypeSpecifications.Count > 0)
            {
                ISpecificationPipeBuilder<SendContext<TMessage>> implementedBuilder = builder.CreateImplementedBuilder();

                foreach (ISpecificationPipeSpecification<SendContext<TMessage>> specification in _implementedMessageTypeSpecifications.Reverse())
                {
                    specification.Apply(implementedBuilder);
                }
            }

            ISpecificationPipeBuilder<SendContext<TMessage>> delegatedBuilder = builder.CreateDelegatedBuilder();

            foreach (ISpecificationPipeSpecification<SendContext<TMessage>> specification in _parentMessageSpecifications)
            {
                specification.Apply(delegatedBuilder);
            }

            foreach (IPipeSpecification<SendContext<TMessage>> specification in _specifications)
            {
                specification.Apply(builder);
            }


            if (!builder.IsImplemented)
            {
                foreach (IPipeSpecification<SendContext> specification in _baseSpecifications)
                {
                    var split = new SplitFilterPipeSpecification<SendContext<TMessage>, SendContext>(specification, MergeContext, FilterContext);

                    split.Apply(builder);
                }
            }
        }

        public IPipe<SendContext<TMessage>> BuildMessagePipe()
        {
            var pipeBuilder = new SpecificationPipeBuilder<SendContext<TMessage>>();

            Apply(pipeBuilder);

            return pipeBuilder.Build();
        }

        public void AddParentMessageSpecification(ISpecificationPipeSpecification<SendContext<TMessage>> parentSpecification)
        {
            _parentMessageSpecifications.Add(parentSpecification);
        }

        public void AddImplementedMessageSpecification<T>(ISpecificationPipeSpecification<SendContext<T>> implementedSpecification)
            where T : class
        {
            var adapter = new ImplementedTypeAdapter<T>(implementedSpecification);

            _implementedMessageTypeSpecifications.Add(adapter);
        }

        static SendContext FilterContext(SendContext<TMessage> context)
        {
            return context;
        }

        static SendContext<TMessage> MergeContext(SendContext<TMessage> input, SendContext context)
        {
            var result = context as SendContext<TMessage>;

            return result ?? new SendContextProxy<TMessage>(context, input.Message);
        }


        class ImplementedTypeAdapter<T> :
            ISpecificationPipeSpecification<SendContext<TMessage>>
            where T : class
        {
            readonly ISpecificationPipeSpecification<SendContext<T>> _specification;

            public ImplementedTypeAdapter(ISpecificationPipeSpecification<SendContext<T>> specification)
            {
                _specification = specification;
            }

            public void Apply(ISpecificationPipeBuilder<SendContext<TMessage>> builder)
            {
                var specification = new MessageSendPipeSplitFilterSpecification<TMessage, T>(_specification);

                specification.Apply(builder);
            }

            public IEnumerable<ValidationResult> Validate()
            {
                yield break;
            }
        }
    }
}