namespace MassTransit.Configuration
{
    using System;


    public class StateMachineScheduleConfigurator<TInstance, TMessage> :
        IScheduleConfigurator<TInstance, TMessage>,
        ScheduleSettings<TInstance, TMessage>
        where TInstance : class, SagaStateMachineInstance
        where TMessage : class
    {
        Action<IEventCorrelationConfigurator<TInstance, TMessage>> _received;

        public StateMachineScheduleConfigurator()
        {
            Delay = TimeSpan.FromSeconds(30);
        }

        public ScheduleSettings<TInstance, TMessage> Settings => this;

        public TimeSpan Delay
        {
            set { DelayProvider = _ => value; }
        }

        public ScheduleDelayProvider<TInstance> DelayProvider { get; set; }

        Action<IEventCorrelationConfigurator<TInstance, TMessage>> IScheduleConfigurator<TInstance, TMessage>.Received
        {
            set => _received = value;
        }

        Action<IEventCorrelationConfigurator<TInstance, TMessage>> ScheduleSettings<TInstance, TMessage>.Received => _received;
    }
}
