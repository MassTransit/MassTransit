namespace MassTransit.TestFramework.Futures;

using System;


public interface ProcessBatchItem
{
    public Guid CorrelationId { get; }
    public string JobNumber { get; }
}
