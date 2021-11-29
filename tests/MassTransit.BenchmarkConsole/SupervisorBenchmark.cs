namespace MassTransit.BenchmarkConsole
{
    using System.Threading.Tasks;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Jobs;
    using Middleware;


    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [SimpleJob(RuntimeMoniker.Net50)]
    [MemoryDiagnoser]
    public class SupervisorBenchmark
    {
        [Benchmark]
        public async Task AddAgentAndStop()
        {
            var supervisor = new Supervisor();

            var provocateur = new Agent();

            provocateur.SetReady();
            supervisor.SetReady();

            supervisor.Add(provocateur);

            await supervisor.Ready;

            await supervisor.Stop();

            await supervisor.Completed;
        }

        [Benchmark]
        public async Task AddAgentWithManagerAndStop()
        {
            var supervisor = new Supervisor();

            var manager = new Supervisor();
            supervisor.Add(manager);

            var provocateur = new Agent();
            manager.Add(provocateur);

            manager.SetReady();
            supervisor.SetReady();
            provocateur.SetReady();

            await supervisor.Ready;

            await supervisor.Stop();

            await supervisor.Completed;
        }
    }
}
