namespace MassTransit.EntityFrameworkCoreIntegration.Tests.Shared
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using NUnit.Framework;
    using Quartz;


    public class QuartzEntityFrameworkTestFixture<TTestDbParameters, TDbContext>
        : EntityFrameworkTestFixture<TTestDbParameters, TDbContext>
        where TTestDbParameters : ITestDbParameters, new()
        where TDbContext : DbContext
    {
        IScheduler _scheduler;
        TimeSpan _testOffset;

        public QuartzEntityFrameworkTestFixture()
        {
            QuartzAddress = new Uri("loopback://localhost/quartz");
            _testOffset = TimeSpan.Zero;
        }

        protected Uri QuartzAddress { get; }

        protected ISendEndpoint QuartzEndpoint { get; set; }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseInMemoryScheduler(out _scheduler);

            base.ConfigureInMemoryBus(configurator);
        }

        protected void AdvanceTime(TimeSpan duration)
        {
            _scheduler.Standby();

            _testOffset += duration;

            _scheduler.Start();
        }

        [OneTimeSetUp]
        public async Task Setup_quartz_service()
        {
            QuartzEndpoint = await GetSendEndpoint(QuartzAddress);

            SystemTime.UtcNow = GetUtcNow;
            SystemTime.Now = GetNow;
        }

        [OneTimeTearDown]
        public void Take_it_down()
        {
            SystemTime.UtcNow = () => DateTimeOffset.UtcNow;
            SystemTime.Now = () => DateTimeOffset.Now;
        }

        DateTimeOffset GetUtcNow()
        {
            return DateTimeOffset.UtcNow + _testOffset;
        }

        DateTimeOffset GetNow()
        {
            return DateTimeOffset.Now + _testOffset;
        }
    }
}
