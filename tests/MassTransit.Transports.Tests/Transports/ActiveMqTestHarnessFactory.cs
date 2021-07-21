namespace MassTransit.Transports.Tests.Transports
{
    using System.Threading.Tasks;
    using ActiveMqTransport.Testing;
    using Testing;


    public class ActiveMqTestHarnessFactory :
        ITestHarnessFactory
    {
        public Task<BusTestHarness> CreateTestHarness()
        {
            return Task.FromResult<BusTestHarness>(new ActiveMqTestHarness());
        }
    }
}
