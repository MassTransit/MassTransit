namespace MassTransit.Configuration
{
    using System;


    public class MessageCorrelationIdFaultEventCorrelationBuilder<TInstance, TData> :
        IEventCorrelationBuilder
        where TData : class
        where TInstance : class, SagaStateMachineInstance
    {
        readonly MassTransitEventCorrelationConfigurator<TInstance, Fault<TData>> _configurator;

        public MessageCorrelationIdFaultEventCorrelationBuilder(SagaStateMachine<TInstance> machine, Event<Fault<TData>> @event,
            IMessageCorrelationId<TData> messageCorrelationId)
        {
            var configurator = new MassTransitEventCorrelationConfigurator<TInstance, Fault<TData>>(machine, @event, null);

            configurator.CorrelateById(x => messageCorrelationId.TryGetCorrelationId(x.Message.Message, out var correlationId)
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
