namespace MassTransit.Configuration
{
    using System.Collections.Generic;
    using System.Threading;
    using Context;
    using Courier.Contracts;
    using Middleware;
    using Observables;
    using RetryPolicies;


    public class ExecuteContextRedeliveryPipeSpecification<TArguments> :
        ExceptionSpecification,
        IRedeliveryConfigurator,
        IPipeSpecification<ExecuteContext<TArguments>>
        where TArguments : class
    {
        readonly RetryObservable _observers;
        RetryPolicyFactory _policyFactory;

        public ExecuteContextRedeliveryPipeSpecification()
        {
            _observers = new RetryObservable();
        }

        public void Apply(IPipeBuilder<ExecuteContext<TArguments>> builder)
        {
            var retryPolicy = _policyFactory(Filter);

            var policy = new ConsumeContextRetryPolicy<ExecuteContext<TArguments>, RetryExecuteContext<TArguments>>(retryPolicy, CancellationToken.None,
                Factory);

            builder.AddFilter(new RedeliveryRetryFilter<ExecuteContext<TArguments>, RoutingSlip>(policy, _observers));
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

        public bool ReplaceMessageId { get; set; }

        static RetryExecuteContext<TArguments> Factory(ExecuteContext<TArguments> context, IRetryPolicy retryPolicy, RetryContext retryContext)
        {
            return context as RetryExecuteContext<TArguments> ?? new RetryExecuteContext<TArguments>(context, retryPolicy, retryContext);
        }
    }
}
