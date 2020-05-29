namespace Automatonymous
{
    using System;


    public interface IScheduleConfigurator<TInstance, TMessage>
        where TInstance : class, SagaStateMachineInstance
        where TMessage : class
    {
        /// <summary>
        /// Sets the message send delay
        /// </summary>
        TimeSpan Delay { set; }

        Action<IEventCorrelationConfigurator<TInstance, TMessage>> Received { set; }
    }
}
