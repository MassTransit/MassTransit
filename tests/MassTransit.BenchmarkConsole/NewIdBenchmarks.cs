namespace MassTransit.BenchmarkConsole
{
    using System;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Configs;
    using BenchmarkDotNet.Environments;
    using BenchmarkDotNet.Jobs;


    public class Config :
        ManualConfig
    {
        public Config()
        {
            // Run with intrinsics disabled
            AddJob(
                Job.Default.WithEnvironmentVariable(new EnvironmentVariable("DOTNET_EnableSSE2", "0")).WithRuntime(CoreRuntime.Core60).AsDefault());

            AddJob(
                Job.Default.WithEnvironmentVariable(new EnvironmentVariable("DOTNET_EnableSSE2", "0")).WithRuntime(CoreRuntime.Core70));

            // Run with intrinsics
            AddJob(
                Job.Default.WithRuntime(CoreRuntime.Core60));
            AddJob(
                Job.Default.WithRuntime(CoreRuntime.Core70));
            AddJob(
                Job.Default.WithRuntime(CoreRuntime.Core31));
        }
    }


    [Config(typeof(Config))]
    [MemoryDiagnoser(false)]
    public class NewIdBenchmarks
    {
        public Guid Guid = Guid.NewGuid();
        public NewId Max = NewId.Next();
        public NewId Min = NewId.Empty;

        [Benchmark]
        public Guid ToGuid()
        {
            return Max.ToGuid();
        }

        [Benchmark]
        public Guid ToSequentialGuid()
        {
            return Max.ToSequentialGuid();
        }

        [Benchmark]
        public byte[] ToByteArray()
        {
            return Max.ToByteArray();
        }

        [Benchmark]
        public NewId FromGuid()
        {
            return NewId.FromGuid(Guid);
        }

        [Benchmark]
        public NewId FromSequentialGuid()
        {
            return NewId.FromSequentialGuid(Guid);
        }

        [Benchmark]
        public string NewIdToString()
        {
            return Max.ToString();
        }

        //[Benchmark]
        //public byte[] GetFormatterArray()
        //{
        //    return Max.GetSequentialFormatterArray();
        //}

        //[Benchmark]
        //public Guid NextGuid()
        //{
        //    return NewId.NextGuid();
        //}

        //[Benchmark]
        //public Guid NextGuidBulk()
        //{
        //    Guid g;
        //    for (int i = 0; i < 100_000; i++)
        //    {
        //        g = NewId.NextGuid();
        //    }
        //    return NewId.NextGuid();
        //}

        //[Benchmark]
        //public Guid NextSequentialGuid()
        //{
        //    return NewId.NextSequentialGuid();
        //}

        //[Benchmark]
        //public Guid NextSequentialGuidBulk()
        //{
        //    Guid g;
        //    for (int i = 0; i < 100_000; i++)
        //    {
        //        g = NewId.NextSequentialGuid();
        //    }
        //    return NewId.NextSequentialGuid();
        //}
    }
}
