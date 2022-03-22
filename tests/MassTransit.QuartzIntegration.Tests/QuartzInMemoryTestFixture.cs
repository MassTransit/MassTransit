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
        QuartzTimeAdjustment _adjustment;
        ISchedulerFactory _schedulerFactory;

        public QuartzInMemoryTestFixture()
        {
            QuartzAddress = new Uri("loopback://localhost/quartz");

            _messageScheduler = new Lazy<IMessageScheduler>(() =>
                new MessageScheduler(new EndpointScheduleMessageProvider(() => GetSendEndpoint(QuartzAddress)), Bus.Topology));
        }

        protected Uri QuartzAddress { get; }

        protected ISendEndpoint QuartzEndpoint { get; set; }

        protected IMessageScheduler Scheduler => _messageScheduler.Value;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseInMemoryScheduler(out _schedulerFactory);

            _adjustment = new QuartzTimeAdjustment(_schedulerFactory);

            base.ConfigureInMemoryBus(configurator);
        }

        protected Task AdvanceTime(TimeSpan duration)
        {
            return _adjustment.AdvanceTime(duration);
        }

        [OneTimeSetUp]
        public async Task Setup_quartz_service()
        {
            QuartzEndpoint = await GetSendEndpoint(QuartzAddress);
        }

        [OneTimeTearDown]
        public void Take_it_down()
        {
            _adjustment?.Dispose();
        }
    }
}
