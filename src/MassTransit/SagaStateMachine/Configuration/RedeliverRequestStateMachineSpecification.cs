namespace MassTransit.Configuration
{
    using System;


    public class RedeliverRequestStateMachineSpecification :
        IRequestStateMachineMissingInstanceConfigurator
    {
        readonly Action<IMissingInstanceRedeliveryConfigurator> _configure;

        public RedeliverRequestStateMachineSpecification(Action<IMissingInstanceRedeliveryConfigurator> configure)
        {
            _configure = configure;
        }

        public IPipe<ConsumeContext<TMessage>> Apply<TInstance, TMessage>(IMissingInstanceConfigurator<TInstance, TMessage> configurator)
            where TInstance : SagaStateMachineInstance
            where TMessage : class
        {
            return configurator.Redeliver(r =>
            {
                r.OnRedeliveryLimitReached(x => x.Fault());

                _configure?.Invoke(r);
            });
        }
    }
}
