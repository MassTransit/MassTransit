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
    using GreenPipes.Filters;
    using GreenPipes.Observers;
    using GreenPipes.Specifications;
    using Pipeline.Filters;


    public class ConsumeContextRetryPipeSpecification :
        ExceptionSpecification,
        IRetryConfigurator,
        IPipeSpecification<ConsumeContext>
    {
        readonly RetryObservable _observers;
        RetryPolicyFactory _policyFactory;

        public ConsumeContextRetryPipeSpecification()
        {
            _observers = new RetryObservable();
        }

        public void Apply(IPipeBuilder<ConsumeContext> builder)
        {
            var retryPolicy = _policyFactory(Filter);

            var contextRetryPolicy = new ConsumeContextRetryPolicy(retryPolicy);

            builder.AddFilter(new RetryFilter<ConsumeContext>(contextRetryPolicy, _observers));
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


    public class ConsumeContextRetryPipeSpecification<TFilter, TContext> :
        ExceptionSpecification,
        IRetryConfigurator,
        IPipeSpecification<TFilter>
        where TFilter : class, ConsumeContext
        where TContext : RetryConsumeContext, TFilter
    {
        readonly Func<TFilter, IRetryPolicy, TContext> _contextFactory;
        readonly RetryObservable _observers;
        RetryPolicyFactory _policyFactory;

        public ConsumeContextRetryPipeSpecification(Func<TFilter, IRetryPolicy, TContext> contextFactory)
        {
            _contextFactory = contextFactory;

            _observers = new RetryObservable();
        }

        public void Apply(IPipeBuilder<TFilter> builder)
        {
            var retryPolicy = _policyFactory(Filter);

            var contextRetryPolicy = new ConsumeContextRetryPolicy<TFilter, TContext>(retryPolicy, _contextFactory);

            builder.AddFilter(new RetryFilter<TFilter>(contextRetryPolicy, _observers));
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
}