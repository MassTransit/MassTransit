namespace MassTransit
{
    using System;


    public interface ExceptionConsumerConsumeContext<out T> :
        ConsumerConsumeContext<T>
        where T : class
    {
        /// <summary>
        /// The exception that was thrown
        /// </summary>
        Exception Exception { get; }

        /// <summary>
        /// The exception info, suitable for inclusion in a fault message
        /// </summary>
        ExceptionInfo ExceptionInfo { get; }
    }
}
