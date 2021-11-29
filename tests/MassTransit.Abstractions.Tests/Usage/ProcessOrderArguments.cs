namespace MassTransit.Abstractions.Tests.Usage
{
    using System;


    public interface ProcessOrderArguments
    {
        Guid OrderId { get; }
    }
}
