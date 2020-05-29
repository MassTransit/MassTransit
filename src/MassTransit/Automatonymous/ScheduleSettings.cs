namespace Automatonymous
{
    using System;


    /// <summary>
    /// The schedule settings, including the default delay for the message
    /// </summary>
    public interface ScheduleSettings<TInstance, TMessage>
        where TInstance : class, SagaStateMachineInstance
        where TMessage : class
    {
        /// <summary>
        /// The delay before the message is sent
        /// </summary>
        TimeSpan Delay { get; }

        /// <summary>
        /// Configure the received correlation
        /// </summary>
        Action<IEventCorrelationConfigurator<TInstance, TMessage>> Received { get; }
    }
}
