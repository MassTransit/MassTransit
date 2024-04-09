namespace MassTransit.TestFramework.Futures;

using System;
using System.Collections.Generic;


public interface BatchCompleted
{
    public Guid CorrelationId { get; }
    public IReadOnlyList<string> ProcessedJobsNumbers { get; }
}
