namespace MassTransit.HangfireIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using Hangfire;
    using Hangfire.MemoryStorage;
    using NUnit.Framework;
    using TestFramework;


    public class HangfireInMemoryTestFixture :
        InMemoryTestFixture
    {
        ISendEndpoint _quartzEndpoint;
        TimeSpan _testOffset;
        readonly string _queueName;
        BackgroundJobServer _server;

        public HangfireInMemoryTestFixture()
        {
            _queueName = "hangfire";
            QuartzAddress = new Uri($"loopback://localhost/{_queueName}");
            _testOffset = TimeSpan.Zero;
        }

        protected Uri QuartzAddress { get; }

        protected ISendEndpoint QuartzEndpoint => _quartzEndpoint;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseHangfireScheduler(_queueName);

            base.ConfigureInMemoryBus(configurator);
        }

        protected void AdvanceTime(TimeSpan duration)
        {
            _testOffset += duration;
        }

        [OneTimeSetUp]
        public async Task Setup_quartz_service()
        {
            GlobalConfiguration.Configuration
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseMemoryStorage();

            _quartzEndpoint = await GetSendEndpoint(QuartzAddress);
            _server = new BackgroundJobServer(new BackgroundJobServerOptions
            {
                Activator = new BusJobActivator(Bus),
                SchedulePollingInterval = TimeSpan.FromSeconds(1),
                ServerCheckInterval = TimeSpan.FromSeconds(1)
            });
        }

        [OneTimeTearDown]
        public void Take_it_down()
        {
            _server.Dispose();
        }


        class BusJobActivator : JobActivator
        {
            readonly IBus _bus;

            public BusJobActivator(IBus bus)
            {
                _bus = bus;
            }

            public override object ActivateJob(Type jobType)
            {
                return Activator.CreateInstance(jobType, _bus);
            }
        }
    }
}
