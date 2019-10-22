namespace MassTransit.PipeConfigurators
{
    using System.Collections.Generic;
    using System.Threading;
    using Context;
    using GreenPipes;
    using GreenPipes.Configurators;
    using GreenPipes.Observers;
    using GreenPipes.Specifications;
    using Pipeline.Filters;


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

            var policy = new ConsumeContextRetryPolicy<ConsumeContext<TMessage>, RetryConsumeContext<TMessage>>(retryPolicy, CancellationToken.None, Factory);

            builder.AddFilter(new RedeliveryRetryFilter<ConsumeContext<TMessage>, TMessage>(policy, _observers));
        }

        static RetryConsumeContext<TMessage> Factory(ConsumeContext<TMessage> context, IRetryPolicy retryPolicy, RetryContext retryContext)
        {
            return context as RetryConsumeContext<TMessage> ?? new RedeliveryRetryConsumeContext<TMessage>(context, retryPolicy, retryContext);
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
