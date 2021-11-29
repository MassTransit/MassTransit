namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Middleware;
    using Observables;
    using RetryPolicies;


    public class ConsumeContextRetryPipeSpecification :
        ExceptionSpecification,
        IRetryConfigurator,
        IPipeSpecification<ConsumeContext>
    {
        readonly CancellationToken _cancellationToken;
        readonly RetryObservable _observers;
        RetryPolicyFactory _policyFactory;

        public ConsumeContextRetryPipeSpecification(CancellationToken cancellationToken = default)
        {
            _observers = new RetryObservable();

            _cancellationToken = cancellationToken;
        }

        public void Apply(IPipeBuilder<ConsumeContext> builder)
        {
            var retryPolicy = _policyFactory(Filter);

            var contextRetryPolicy = new ConsumeContextRetryPolicy(retryPolicy, _cancellationToken);

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
        readonly CancellationToken _cancellationToken;
        readonly Func<TFilter, IRetryPolicy, RetryContext, TContext> _contextFactory;
        readonly RetryObservable _observers;
        RetryPolicyFactory _policyFactory;

        public ConsumeContextRetryPipeSpecification(Func<TFilter, IRetryPolicy, RetryContext, TContext> contextFactory,
            CancellationToken cancellationToken = default)
        {
            _contextFactory = contextFactory;

            _observers = new RetryObservable();
            _cancellationToken = cancellationToken;
        }

        public void Apply(IPipeBuilder<TFilter> builder)
        {
            var retryPolicy = _policyFactory(Filter);

            var contextRetryPolicy = new ConsumeContextRetryPolicy<TFilter, TContext>(retryPolicy, _cancellationToken, _contextFactory);

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
