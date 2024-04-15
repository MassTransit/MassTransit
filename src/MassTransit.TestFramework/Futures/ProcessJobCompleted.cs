namespace MassTransit.TestFramework.Futures;

using System;


public interface ProcessJobCompleted
{
    public Guid CorrelationId { get; }
    public string JobNumber { get; }
}
