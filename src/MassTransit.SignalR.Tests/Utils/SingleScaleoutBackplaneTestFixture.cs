namespace MassTransit.SignalR.Tests
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.SignalR;
    using NUnit.Framework;
    using Testing;


    public abstract class SingleScaleoutBackplaneTestFixture<THub> : MassTransitHubLifetimeTestFixture<THub>
        where THub : Hub
    {
        protected override BusTestHarness Harness { get; set; }

        protected SignalRBackplaneConsumersTestHarness<THub> BackplaneHarness { get; private set; }

        [SetUp]
        public async Task Setup()
        {
            Harness = new InMemoryTestHarness();

            BackplaneHarness = RegisterBusEndpoint();

            await Harness.Start();

            // Need the bus to be started before we can set the manager for our Consumer Factory (so it can be injected into the consumers on creation);
            var hubLifetimeManager = CreateLifetimeManager();
            BackplaneHarness.SetHubLifetimeManager(hubLifetimeManager);
        }

        [TearDown]
        public async Task Teardown()
        {
            await Harness.Stop();
        }
    }
}
