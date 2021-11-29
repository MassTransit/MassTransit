namespace MassTransit.BenchmarkConsole.Throughput
{
    using System.Threading.Tasks;


    public class BenchmarkFilter :
        IFilter<TestContext>
    {
        public Task Send(TestContext context, IPipe<TestContext> next)
        {
            return next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
        }
    }
}
