namespace MassTransit.Contracts
{
    using System;


    public interface EventContext :
        PipeContext
    {
        /// <summary>
        /// The timestamp at which the command was sent
        /// </summary>
        DateTime Timestamp { get; }
    }


    public interface EventContext<out T> :
        EventContext
        where T : class
    {
        /// <summary>
        /// The event object
        /// </summary>
        T Event { get; }
    }
}
