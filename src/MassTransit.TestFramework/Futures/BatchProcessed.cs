namespace MassTransit.TestFramework.Futures;

using System.Collections.Generic;


public interface BatchProcessed
{
    public IReadOnlyList<string> SuccessfulJobNumbers { get; }
}
