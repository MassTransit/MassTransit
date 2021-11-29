namespace MassTransit
{
    using System;


    public interface IScheduleConfigurator<TInstance, TMessage>
        where TInstance : class, SagaStateMachineInstance
        where TMessage : class
    {
        /// <summary>
        /// Set a fixed message delay, which is applied to all scheduled messages unless
        /// overriden by the .Schedule method.
        /// </summary>
        TimeSpan Delay { set; }

        /// <summary>
        /// Set a dynamic message delay provider, which uses the instance to determine the delay
        /// unless overriden by the .Schedule method.
        /// </summary>
        ScheduleDelayProvider<TInstance> DelayProvider { set; }

        /// <summary>
        /// Configure the behavior of the Received event, the same was Events are configured on
        /// the state machine.
        /// </summary>
        Action<IEventCorrelationConfigurator<TInstance, TMessage>> Received { set; }
    }
}
