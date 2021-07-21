namespace MassTransit.Transports.Tests.Transports
{
    using System.Threading.Tasks;
    using Testing;


    public interface ITestHarnessFactory
    {
        Task<BusTestHarness> CreateTestHarness();
    }
}
