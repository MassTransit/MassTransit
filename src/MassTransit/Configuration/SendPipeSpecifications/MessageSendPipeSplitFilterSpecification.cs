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
    using System.Collections.Generic;
    using GreenPipes;
    using GreenPipes.Filters;
    using PipeBuilders;


    public class MessageSendPipeSplitFilterSpecification<TMessage, T> :
        ISpecificationPipeSpecification<SendContext<TMessage>>
        where TMessage : class
        where T : class
    {
        readonly ISpecificationPipeSpecification<SendContext<T>> _specification;

        public MessageSendPipeSplitFilterSpecification(ISpecificationPipeSpecification<SendContext<T>> specification)
        {
            _specification = specification;
        }

        public void Apply(ISpecificationPipeBuilder<SendContext<TMessage>> builder)
        {
            var splitBuilder = new Builder(builder);

            _specification.Apply(splitBuilder);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_specification == null)
                yield return this.Failure("Specification", "must not be null");
        }


        class Builder :
            ISpecificationPipeBuilder<SendContext<T>>
        {
            readonly ISpecificationPipeBuilder<SendContext<TMessage>> _builder;

            public Builder(ISpecificationPipeBuilder<SendContext<TMessage>> builder)
            {
                _builder = builder;
            }

            public void AddFilter(IFilter<SendContext<T>> filter)
            {
                var splitFilter = new SplitFilter<SendContext<TMessage>, SendContext<T>>(filter, ContextProvider, InputContextProvider);

                _builder.AddFilter(splitFilter);
            }

            public bool IsDelegated => _builder.IsDelegated;
            public bool IsImplemented => _builder.IsImplemented;

            public ISpecificationPipeBuilder<SendContext<T>> CreateDelegatedBuilder()
            {
                return new ChildSpecificationPipeBuilder<SendContext<T>>(this, IsImplemented, true);
            }

            public ISpecificationPipeBuilder<SendContext<T>> CreateImplementedBuilder()
            {
                return new ChildSpecificationPipeBuilder<SendContext<T>>(this, true, IsDelegated);
            }

            SendContext<TMessage> ContextProvider(SendContext<TMessage> context, SendContext<T> splitContext)
            {
                return context;
            }

            SendContext<T> InputContextProvider(SendContext<TMessage> context)
            {
                return context as SendContext<T>;
            }
        }
    }
}