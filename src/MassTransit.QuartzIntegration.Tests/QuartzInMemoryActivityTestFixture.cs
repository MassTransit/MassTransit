namespace MassTransit.QuartzIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Quartz;
    using TestFramework;


    public abstract class QuartzInMemoryActivityTestFixture :
        InMemoryActivityTestFixture
    {
        ISendEndpoint _quartzEndpoint;
        TimeSpan _testOffset;

        protected QuartzInMemoryActivityTestFixture()
        {
            QuartzAddress = new Uri("loopback://localhost/quartz");
            _testOffset = TimeSpan.Zero;
        }

        protected Uri QuartzAddress { get; }

        protected ISendEndpoint QuartzEndpoint => _quartzEndpoint;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseInMemoryScheduler();

            base.ConfigureInMemoryBus(configurator);
        }

        protected void AdvanceTime(TimeSpan duration)
        {
            _testOffset = _testOffset + duration;
        }

        [OneTimeSetUp]
        public async Task Setup_quartz_service()
        {
            _quartzEndpoint = await GetSendEndpoint(QuartzAddress);

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
