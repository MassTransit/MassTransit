namespace MassTransit.Initializers.Variables
{
    using System;


    public interface TimestampContext
    {
        DateTime Timestamp { get; }
    }
}