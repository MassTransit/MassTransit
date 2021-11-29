namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using Middleware;
    using Util;


    public class MissingInstanceRedeliveryConfigurator<TSaga, TMessage> :
        ExceptionSpecification,
        IMissingInstanceRedeliveryConfigurator<TSaga, TMessage>,
        ISpecification
        where TSaga : SagaStateMachineInstance
        where TMessage : class
    {
        readonly IMissingInstanceConfigurator<TSaga, TMessage> _configurator;
        IPipe<ConsumeContext<TMessage>> _finalPipe;
        RetryPolicyFactory _policyFactory;

        public MissingInstanceRedeliveryConfigurator(IMissingInstanceConfigurator<TSaga, TMessage> configurator)
        {
            _configurator = configurator;

            _finalPipe = configurator.Discard();
        }

        public void SetRetryPolicy(RetryPolicyFactory factory)
        {
            _policyFactory = factory;
        }

        public void OnRedeliveryLimitReached(Func<IMissingInstanceConfigurator<TSaga, TMessage>, IPipe<ConsumeContext<TMessage>>> configure)
        {
            _finalPipe = configure(_configurator) ?? _configurator.Discard();
        }

        public ConnectHandle ConnectRetryObserver(IRetryObserver observer)
        {
            return new EmptyConnectHandle();
        }

        public bool ReplaceMessageId { get; set; } = true;

        public IEnumerable<ValidationResult> Validate()
        {
            if (_policyFactory == null)
                yield return this.Failure("RetryPolicy", "must not be null");
        }

        public IPipe<ConsumeContext<TMessage>> Build()
        {
            var retryPolicy = _policyFactory(Filter);

            var options = ReplaceMessageId ? RedeliveryOptions.ReplaceMessageId : RedeliveryOptions.None;

            return new MissingInstanceRedeliveryPipe<TSaga, TMessage>(retryPolicy, _finalPipe, options);
        }
    }
}
