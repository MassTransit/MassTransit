namespace MassTransit.Configuration
{
    using System.Collections.Generic;
    using System.Threading;
    using Middleware;
    using Observables;
    using RetryPolicies;


    public class RedeliveryRetryPipeSpecification<TMessage> :
        ExceptionSpecification,
        IRedeliveryConfigurator,
        IPipeSpecification<ConsumeContext<TMessage>>
        where TMessage : class
    {
        readonly RetryObservable _observers;
        readonly IRedeliveryPipeSpecification _redeliveryPipeSpecification;
        RetryPolicyFactory _policyFactory;

        public RedeliveryRetryPipeSpecification(IRedeliveryPipeSpecification redeliveryPipeSpecification)
        {
            _redeliveryPipeSpecification = redeliveryPipeSpecification;
            _observers = new RetryObservable();
        }

        public void Apply(IPipeBuilder<ConsumeContext<TMessage>> builder)
        {
            var retryPolicy = _policyFactory(Filter);

            var policy = new ConsumeContextRetryPolicy<ConsumeContext<TMessage>, RetryConsumeContext<TMessage>>(retryPolicy, CancellationToken.None, Factory);

            builder.AddFilter(new RedeliveryRetryFilter<ConsumeContext<TMessage>, TMessage>(policy, _observers));
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

        public ConnectHandle ConnectRetryObserver(IRetryObserver observer)
        {
            return _observers.Connect(observer);
        }

        public bool ReplaceMessageId
        {
            set
            {
                if (value)
                    _redeliveryPipeSpecification.Options |= RedeliveryOptions.ReplaceMessageId;
                else
                    _redeliveryPipeSpecification.Options &= ~RedeliveryOptions.ReplaceMessageId;
            }
        }

        static RetryConsumeContext<TMessage> Factory(ConsumeContext<TMessage> context, IRetryPolicy retryPolicy, RetryContext retryContext)
        {
            return context as RetryConsumeContext<TMessage> ?? new RedeliveryRetryConsumeContext<TMessage>(context, retryPolicy, retryContext);
        }
    }
}
