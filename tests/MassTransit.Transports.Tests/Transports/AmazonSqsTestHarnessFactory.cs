namespace MassTransit.Transports.Tests.Transports
{
    using System.Threading.Tasks;
    using Testing;


    public class AmazonSqsTestHarnessFactory :
        ITestHarnessFactory
    {
        public Task<BusTestHarness> CreateTestHarness()
        {
            return Task.FromResult<BusTestHarness>(new AmazonSqsTestHarness());
        }
    }
}
