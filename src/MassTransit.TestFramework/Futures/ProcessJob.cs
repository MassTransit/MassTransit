namespace MassTransit.TestFramework.Futures;

using System;


public interface ProcessJob
{
    public Guid CorrelationId { get; }
    public string ClientNumber { get; }
}
