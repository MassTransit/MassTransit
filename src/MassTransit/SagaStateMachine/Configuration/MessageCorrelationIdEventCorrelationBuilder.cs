namespace MassTransit.Configuration
{
    using System;


    public class MessageCorrelationIdEventCorrelationBuilder<TInstance, TData> :
        IEventCorrelationBuilder
        where TData : class
        where TInstance : class, SagaStateMachineInstance
    {
        readonly MassTransitEventCorrelationConfigurator<TInstance, TData> _configurator;

        public MessageCorrelationIdEventCorrelationBuilder(SagaStateMachine<TInstance> machine, Event<TData> @event,
            IMessageCorrelationId<TData> messageCorrelationId)
        {
            var configurator = new MassTransitEventCorrelationConfigurator<TInstance, TData>(machine, @event, null);

            configurator.CorrelateById(x => messageCorrelationId.TryGetCorrelationId(x.Message, out var correlationId)
                ? correlationId
                : throw new ArgumentException($"The message {TypeCache<TData>.ShortName} did not have a correlationId"));

            _configurator = configurator;
        }

        public EventCorrelation Build()
        {
            return _configurator.Build();
        }
    }
}
