namespace MassTransit.BenchmarkConsole
{
    using System;
    using BenchmarkDotNet.Running;


    class Program
    {
        static void Main()
        {
            Console.WriteLine("MassTransit Benchmark");
            Console.WriteLine();

            //BenchmarkRunner.Run<ChannelBenchmark>();
            BenchmarkRunner.Run<ConcurrentChannelBenchmark>();
        }
    }
}
