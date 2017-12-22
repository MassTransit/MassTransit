// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.PipeConfigurators
{
    using System;
    using System.Collections.Generic;
    using Context;
    using GreenPipes;
    using GreenPipes.Configurators;
    using GreenPipes.Observers;
    using GreenPipes.Specifications;
    using Pipeline.Filters;
    using Util;


    public class RedeliveryRetryPipeSpecification<TMessage> :
        ExceptionSpecification,
        IRetryConfigurator,
        IPipeSpecification<ConsumeContext<TMessage>>
        where TMessage : class
    {
        readonly RetryObservable _observers;
        RetryPolicyFactory _policyFactory;

        public RedeliveryRetryPipeSpecification()
        {
            _observers = new RetryObservable();
        }

        public void Apply(IPipeBuilder<ConsumeContext<TMessage>> builder)
        {
            var retryPolicy = _policyFactory(Filter);

            var policy = new ConsumeContextRetryPolicy<ConsumeContext<TMessage>, RetryConsumeContext<TMessage>>(retryPolicy,
                x => x as RetryConsumeContext<TMessage> ?? new RedeliveryRetryConsumeContext<TMessage>(x));

            builder.AddFilter(new RedeliveryRetryFilter<TMessage>(policy, _observers));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_policyFactory == null)
                yield return this.Failure("RetryPolicy", "must not be null");
        }

        public void SetRetryPolicy(RetryPolicyFactory factory)
        {
            _policyFactory = factory;
        }

        ConnectHandle IRetryObserverConnector.ConnectRetryObserver(IRetryObserver observer)
        {
            return _observers.Connect(observer);
        }
    }


    public class RedeliveryRetryConsumeContext<T> :
        RetryConsumeContext<T>
        where T : class
    {
        public RedeliveryRetryConsumeContext(ConsumeContext<T> context)
            : base(context)
        {
        }

        public override TContext CreateNext<TContext>()
        {
            return this as TContext
                ?? throw new ArgumentException($"The context type is not valid: {TypeMetadataCache<T>.ShortName}");
        }
    }
}