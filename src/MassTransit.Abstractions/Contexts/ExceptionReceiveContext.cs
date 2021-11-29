namespace MassTransit
{
    using System;


    public interface ExceptionReceiveContext :
        ReceiveContext
    {
        /// <summary>
        /// The exception that was thrown
        /// </summary>
        Exception Exception { get; }

        /// <summary>
        /// The time at which the exception was thrown
        /// </summary>
        DateTime ExceptionTimestamp { get; }

        /// <summary>
        /// The exception info, suitable for inclusion in a fault message
        /// </summary>
        ExceptionInfo ExceptionInfo { get; }
    }
}
