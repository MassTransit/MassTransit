namespace MassTransit.Contracts
{
    using System;


    /// <summary>
    /// When the time allowed for the request has expired, this is sent
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    public interface RequestExpired<out TRequest>
        where TRequest : class
    {
        Guid RequestId { get; }

        DateTime Timestamp { get; }
    }
}