namespace MassTransit.TestFramework.Futures;

using System;
using System.Collections.Generic;


public interface BatchRequest
{
    public DateTime? BatchExpiry { get; }
    public Guid CorrelationId { get; }
    public IReadOnlyList<string> JobNumbers { get; }
}
