namespace MassTransit.Contracts
{
    using System;


    /// <summary>
    /// If a request faults, sent to signify the faulted request
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    public interface RequestFaulted<out TRequest>
        where TRequest : class
    {
        Guid RequestId { get; }

        DateTime Timestamp { get; }

        TRequest Request { get; }

        /// <summary>
        /// The exception information that occurred
        /// </summary>
        ExceptionInfo[] Exceptions { get; }
    }
}