namespace MassTransit.HangfireIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using Hangfire;
    using Hangfire.MemoryStorage;
    using NUnit.Framework;
    using Scheduling;
    using TestFramework;


    public class HangfireInMemoryTestFixture :
        InMemoryTestFixture
    {
        readonly string _queueName;
        readonly Lazy<IMessageScheduler> _scheduler;

        static HangfireInMemoryTestFixture()
        {
            GlobalConfiguration.Configuration
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseMemoryStorage();
        }

        public HangfireInMemoryTestFixture()
        {
            _queueName = "hangfire";
            HangfireAddress = new Uri($"queue:{_queueName}");

            _scheduler = new Lazy<IMessageScheduler>(() =>
                new MessageScheduler(new EndpointScheduleMessageProvider(() => GetSendEndpoint(HangfireAddress)), Bus.Topology));
        }

        protected Uri HangfireAddress { get; }

        protected ISendEndpoint HangfireEndpoint { get; private set; }

        protected IMessageScheduler Scheduler => _scheduler.Value;

        protected JobStorage Storage => JobStorage.Current;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseHangfireScheduler(_queueName, cfg =>
            {
                cfg.ServerCheckInterval = TimeSpan.FromSeconds(1);
                cfg.SchedulePollingInterval = TimeSpan.FromSeconds(1);
            });

            base.ConfigureInMemoryBus(configurator);
        }

        [OneTimeSetUp]
        public async Task Setup_quartz_service()
        {
            HangfireEndpoint = await GetSendEndpoint(HangfireAddress);
        }

        [OneTimeTearDown]
        public void Take_it_down()
        {
        }
    }
}
