namespace MassTransit.Contracts
{
    using System;


    /// <summary>
    /// Returned when a request cancellation is requested, but the request has already completed and can no
    /// longer be canceled.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    public interface RequestAlreadyCompleted<out TRequest>
        where TRequest : class
    {
        Guid RequestId { get; }

        DateTime Timestamp { get; }

        TRequest Request { get; }
    }
}