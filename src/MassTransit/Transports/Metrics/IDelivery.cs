namespace MassTransit.Transports.Metrics
{
    using System;


    public interface IDelivery :
        IDisposable
    {
        long Id { get; }
    }
}
