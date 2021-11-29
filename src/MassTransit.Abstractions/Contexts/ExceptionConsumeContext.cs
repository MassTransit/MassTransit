namespace MassTransit
{
    using System;


    public interface ExceptionConsumeContext :
        ConsumeContext
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


    public interface ExceptionConsumeContext<out T> :
        ExceptionConsumeContext,
        ConsumeContext<T>
        where T : class
    {
    }
}
