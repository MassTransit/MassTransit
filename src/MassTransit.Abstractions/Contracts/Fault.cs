namespace MassTransit
{
    using System;


    /// <summary>
    /// A faulted message, published when a message consumer fails to process the message
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface Fault<out T> :
        Fault
    {
        /// <summary>
        /// The message that faulted
        /// </summary>
        T Message { get; }
    }


    /// <summary>
    /// Published (or sent, if part of a request/response conversation) when a fault occurs during message
    /// processing
    /// </summary>
    public interface Fault
    {
        /// <summary>
        /// Identifies the fault that was generated
        /// </summary>
        Guid FaultId { get; }

        /// <summary>
        /// The messageId that faulted
        /// </summary>
        Guid? FaultedMessageId { get; }

        /// <summary>
        /// When the fault was produced
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// The exception information that occurred
        /// </summary>
        ExceptionInfo[] Exceptions { get; }

        /// <summary>
        /// The host information was the fault occurred
        /// </summary>
        HostInfo Host { get; }

        /// <summary>
        /// The faulted message supported types, from the original message envelope
        /// </summary>
        string[] FaultMessageTypes { get; }
    }
}
