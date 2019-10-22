namespace MassTransit.Conductor.Contexts
{
    using System;


    public interface RequestClientContext
    {
        Guid ClientId { get; }
        Guid RequestId { get; }
    }
}
