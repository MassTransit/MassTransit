namespace MassTransit.Configuration
{
    using System;


    public partial class StateMachineInterfaceType<TInstance, TData>
    {
        public class MessageCorrelationIdFaultEventCorrelationBuilder :
            IEventCorrelationBuilder
        {
            readonly StateMachineInterfaceType<TInstance, Fault<TData>>.MassTransitEventCorrelationConfigurator _configurator;

            public MessageCorrelationIdFaultEventCorrelationBuilder(SagaStateMachine<TInstance> machine, Event<Fault<TData>> @event,
                IMessageCorrelationId<TData> messageCorrelationId)
            {
                var configurator = new StateMachineInterfaceType<TInstance, Fault<TData>>.MassTransitEventCorrelationConfigurator(machine, @event, null);

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
}
