namespace MassTransit.BenchmarkConsole
{
    using System;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Jobs;


    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [SimpleJob(RuntimeMoniker.Net60)]
    [SimpleJob(RuntimeMoniker.Net80)]
    [MemoryDiagnoser]
    [GcServer(true)]
    [GcForce]
    public class Benchmarker
    {
        [Benchmark(Baseline = true, Description = "Next")]
        public NewId GetNext()
        {
            return NewId.Next();
        }

        [Benchmark(Description = "Next(batch)", OperationsPerInvoke = 100)]
        public NewId[] GetNextBatch()
        {
            return NewId.Next(100);
        }

        [Benchmark(Description = "NextGuid")]
        public Guid GetNextGuid()
        {
            return NewId.NextGuid();
        }

        [Benchmark(Description = "NextSequentialGuid")]
        public Guid GetNextSequentialGuid()
        {
            return NewId.NextSequentialGuid();
        }
    }
}
