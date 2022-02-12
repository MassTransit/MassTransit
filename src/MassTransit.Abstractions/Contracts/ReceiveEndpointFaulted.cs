namespace MassTransit
{
    using System;


    public interface ReceiveEndpointFaulted :
        ReceiveEndpointEvent
    {
        Exception? Exception { get; }
    }
}
