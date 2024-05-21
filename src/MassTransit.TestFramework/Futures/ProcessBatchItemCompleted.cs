namespace MassTransit.TestFramework.Futures;

using System;


public interface ProcessBatchItemCompleted
{
    public Guid CorrelationId { get; }
    public string JobNumber { get; }
}
