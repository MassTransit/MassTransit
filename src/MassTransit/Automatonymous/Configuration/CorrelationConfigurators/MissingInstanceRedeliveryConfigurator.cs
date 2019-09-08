namespace Automatonymous.CorrelationConfigurators
{
    using System;
    using System.Collections.Generic;
    using GreenPipes;
    using GreenPipes.Configurators;
    using GreenPipes.Specifications;
    using GreenPipes.Util;
    using MassTransit;
    using Pipeline;


    public class MissingInstanceRedeliveryConfigurator<TInstance, TData> :
        ExceptionSpecification,
        IMissingInstanceRedeliveryConfigurator<TInstance, TData>,
        ISpecification
        where TInstance : SagaStateMachineInstance
        where TData : class
    {
        readonly IMissingInstanceConfigurator<TInstance, TData> _configurator;
        RetryPolicyFactory _policyFactory;
        IPipe<ConsumeContext<TData>> _finalPipe;

        public MissingInstanceRedeliveryConfigurator(IMissingInstanceConfigurator<TInstance, TData> configurator)
        {
            _configurator = configurator;

            _finalPipe = configurator.Discard();
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

        public void OnRedeliveryLimitReached(Func<IMissingInstanceConfigurator<TInstance, TData>, IPipe<ConsumeContext<TData>>> configure)
        {
            _finalPipe = configure(_configurator) ?? _configurator.Discard();
        }

        ConnectHandle IRetryObserverConnector.ConnectRetryObserver(IRetryObserver observer)
        {
            return new EmptyConnectHandle();
        }

        public IPipe<ConsumeContext<TData>> Build()
        {
            var retryPolicy = _policyFactory(Filter);

            return new MissingInstanceRedeliveryPipe<TInstance, TData>(retryPolicy, _finalPipe);
        }
    }
}
