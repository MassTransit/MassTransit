namespace MassTransit.SignalR.Tests.Utils
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.SignalR;
    using NUnit.Framework;
    using Testing;


    public abstract class DoubleScaleoutBackplaneTestFixture<THub> :
        MassTransitHubLifetimeTestFixture<THub>
        where THub : Hub
    {
        protected override BusTestHarness Harness { get; set; }

        protected SignalRBackplaneConsumersTestHarness<THub> Backplane1Harness { get; private set; }
        protected SignalRBackplaneConsumersTestHarness<THub> Backplane2Harness { get; private set; }

        [SetUp]
        public async Task Setup()
        {
            Harness = new InMemoryTestHarness();

            Backplane1Harness = RegisterBusEndpoint("receiveEndpoint1");
            Backplane2Harness = RegisterBusEndpoint("receiveEndpoint2");

            await Harness.Start();

            // Need the bus to be started before we can set the manager for our Consumer Factory (so it can be injected into the consumers on creation);
            Backplane1Harness.SetHubLifetimeManager(CreateLifetimeManager());
            Backplane2Harness.SetHubLifetimeManager(CreateLifetimeManager());
        }

        [TearDown]
        public async Task Teardown()
        {
            await Harness.Stop();
        }
    }
}
