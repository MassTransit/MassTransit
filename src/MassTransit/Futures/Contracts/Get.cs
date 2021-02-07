namespace MassTransit.Futures.Contracts
{
    using System;
    using MassTransit;


    /// <summary>
    /// Sent by a client to get a future value by type/ID
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface Get<T> :
        CorrelatedBy<Guid>
        where T : class
    {
    }
}