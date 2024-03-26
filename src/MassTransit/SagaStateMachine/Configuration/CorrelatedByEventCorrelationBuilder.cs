namespace MassTransit.Configuration
{
    using System;


    public class CorrelatedByEventCorrelationBuilder<TInstance, TData> :
        IEventCorrelationBuilder
        where TData : class, CorrelatedBy<Guid>
        where TInstance : class, SagaStateMachineInstance
    {
        readonly StateMachineInterfaceType<TInstance, TData>.MassTransitEventCorrelationConfigurator _configurator;

        public CorrelatedByEventCorrelationBuilder(SagaStateMachine<TInstance> machine, Event<TData> @event)
        {
            var configurator = new StateMachineInterfaceType<TInstance, TData>.MassTransitEventCorrelationConfigurator(machine, @event, null);
            configurator.CorrelateById(x => x.Message.CorrelationId);

            _configurator = configurator;
        }

        public EventCorrelation Build()
        {
            return _configurator.Build();
        }
    }
}
