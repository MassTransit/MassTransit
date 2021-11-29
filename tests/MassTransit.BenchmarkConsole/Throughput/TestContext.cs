namespace MassTransit.BenchmarkConsole.Throughput
{
    using System;


    public interface TestContext :
        PipeContext
    {
        Guid CorrelationId { get; }

        int Attempts { get; set; }
    }
}
