namespace MassTransit.QuartzIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;


    public abstract class QuartzInMemoryActivityTestFixture :
        InMemoryActivityTestFixture
    {
        protected QuartzInMemoryActivityTestFixture()
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
