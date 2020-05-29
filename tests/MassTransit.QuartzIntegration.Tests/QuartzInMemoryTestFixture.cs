namespace MassTransit.QuartzIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Quartz;
    using Scheduling;
    using TestFramework;


    public class QuartzInMemoryTestFixture :
        InMemoryTestFixture
    {
        readonly Lazy<IMessageScheduler> _messageScheduler;
        IScheduler _scheduler;
        TimeSpan _testOffset;

        public QuartzInMemoryTestFixture()
        {
            QuartzAddress = new Uri("loopback://localhost/quartz");
            _testOffset = TimeSpan.Zero;

            _messageScheduler = new Lazy<IMessageScheduler>(() =>
                new MessageScheduler(new EndpointScheduleMessageProvider(() => GetSendEndpoint(QuartzAddress)), Bus.Topology));
        }

        protected Uri QuartzAddress { get; }

        protected ISendEndpoint QuartzEndpoint { get; set; }

        protected IMessageScheduler Scheduler => _messageScheduler.Value;

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
