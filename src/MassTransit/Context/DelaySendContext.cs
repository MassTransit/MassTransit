namespace MassTransit.Context
{
    using System;


    public interface DelaySendContext
    {
        TimeSpan? Delay { get; set; }
    }
}
