namespace MassTransit.Transports.Tests.Transports
{
    using System;
    using System.Threading.Tasks;
    using Testing;


    public class ArtemisTestHarnessFactory :
        ITestHarnessFactory
    {
        public Task<BusTestHarness> CreateTestHarness()
        {
            var harness = new ActiveMqTestHarness
            {
                HostAddress = new Uri("activemq://localhost:61618"),
                AdminPort = 8163,
            };

            harness.OnConfigureActiveMqBus += cfg =>
            {
                cfg.EnableArtemisCompatibility();
            };

            return Task.FromResult<BusTestHarness>(harness);
        }
    }
}
