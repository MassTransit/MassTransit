namespace MassTransit.Transports.Tests.Transports
{
    using System.Threading.Tasks;
    using Testing;


    public class InMemoryTestHarnessFactory :
        ITestHarnessFactory
    {
        public Task<BusTestHarness> CreateTestHarness()
        {
            return Task.FromResult<BusTestHarness>(new InMemoryTestHarness());
        }
    }
}
