namespace MassTransit.TestFramework.Futures;

using System;
using System.Collections.Generic;


public interface BatchFaulted
{
    public Guid CorrelationId { get; }
    public IReadOnlyList<string> ProcessedJobsNumbers { get; }
}
