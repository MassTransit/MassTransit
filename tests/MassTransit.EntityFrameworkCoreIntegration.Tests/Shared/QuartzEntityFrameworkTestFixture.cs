namespace MassTransit.EntityFrameworkCoreIntegration.Tests.Shared
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using NUnit.Framework;


    public class QuartzEntityFrameworkTestFixture<TTestDbParameters, TDbContext>
        : EntityFrameworkTestFixture<TTestDbParameters, TDbContext>
        where TTestDbParameters : ITestDbParameters, new()
        where TDbContext : DbContext
    {
        public QuartzEntityFrameworkTestFixture()
        {
            QuartzAddress = new Uri("loopback://localhost/quartz");
        }

        protected Uri QuartzAddress { get; }

        protected ISendEndpoint QuartzEndpoint { get; set; }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseInMemoryScheduler();

            base.ConfigureInMemoryBus(configurator);
        }

        [OneTimeSetUp]
        public async Task Setup_quartz_service()
        {
            QuartzEndpoint = await GetSendEndpoint(QuartzAddress);
        }
    }
}
