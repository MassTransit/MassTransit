namespace MassTransit.HangfireIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using Hangfire;
    using Hangfire.MemoryStorage;
    using NUnit.Framework;
    using TestFramework;


    public abstract class HangfireInMemoryActivityTestFixture :
        InMemoryActivityTestFixture
    {
        readonly string _queueName;

        static HangfireInMemoryActivityTestFixture()
        {
            GlobalConfiguration.Configuration
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseMemoryStorage();
        }

        protected HangfireInMemoryActivityTestFixture()
        {
            _queueName = "hangfire";
            HangfireAddress = new Uri($"queue:{_queueName}");
        }

        protected Uri HangfireAddress { get; }

        protected ISendEndpoint HangfireEndpoint { get; private set; }

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
