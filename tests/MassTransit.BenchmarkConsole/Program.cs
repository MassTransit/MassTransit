namespace MassTransit.BenchmarkConsole
{
    using System;
    using BenchmarkDotNet.Running;


    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("MassTransit Benchmark");
            Console.WriteLine();

          //  BenchmarkRunner.Run<SendBenchmark>();

            //          BenchmarkRunner.Run<SupervisorBenchmark>();

          //  BenchmarkRunner.Run<JsonSerializationBenchmark>();

            BenchmarkRunner.Run<MediatorBenchmark>();
        }
    }
}
