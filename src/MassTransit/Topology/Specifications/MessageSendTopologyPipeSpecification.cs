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
namespace MassTransit.Topology.Specifications
{
    using System.Collections.Generic;
    using GreenPipes;
    using PipeBuilders;


    public class MessageSendTopologyPipeSpecification<TMessage> :
        ISpecificationPipeSpecification<SendContext<TMessage>>
        where TMessage : class
    {
        readonly IMessageSendTopology<TMessage> _messageSendTopology;

        public MessageSendTopologyPipeSpecification(IMessageSendTopology<TMessage> messageSendTopology)
        {
            _messageSendTopology = messageSendTopology;
        }

        public void Apply(ISpecificationPipeBuilder<SendContext<TMessage>> builder)
        {
            var typeBuilder = new Builder(builder);

            _messageSendTopology.Apply(typeBuilder);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }


        class Builder :
            ITopologyPipeBuilder<SendContext<TMessage>>
        {
            readonly ISpecificationPipeBuilder<SendContext<TMessage>> _builder;

            public Builder(ISpecificationPipeBuilder<SendContext<TMessage>> builder)
            {
                _builder = builder;
            }

            public void AddFilter(IFilter<SendContext<TMessage>> filter)
            {
                _builder.AddFilter(filter);
            }

            public bool IsDelegated => _builder.IsDelegated;
            public bool IsImplemented => _builder.IsImplemented;

            public ITopologyPipeBuilder<SendContext<TMessage>> CreateDelegatedBuilder()
            {
                return new ChildBuilder<SendContext<TMessage>>(this, IsImplemented, true);
            }


            class ChildBuilder<T> :
                ITopologyPipeBuilder<T>
                where T : class, PipeContext
            {
                readonly ITopologyPipeBuilder<T> _builder;

                public ChildBuilder(ITopologyPipeBuilder<T> builder, bool isImplemented, bool isDelegated)
                {
                    _builder = builder;

                    IsDelegated = isDelegated;
                    IsImplemented = isImplemented;
                }

                public void AddFilter(IFilter<T> filter)
                {
                    _builder.AddFilter(filter);
                }

                public bool IsDelegated { get; }

                public bool IsImplemented { get; }

                public ITopologyPipeBuilder<T> CreateDelegatedBuilder()
                {
                    return new ChildBuilder<T>(this, IsImplemented, true);
                }
            }
        }
    }
}