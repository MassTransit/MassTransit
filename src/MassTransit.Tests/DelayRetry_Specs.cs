namespace MassTransit.Tests
{
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class DelayRetry_Specs :
        InMemoryTestFixture
    {
        [Test]
        public void FirstTestName()
        {
            
        }

        protected override void ConfigureBus(IInMemoryBusFactoryConfigurator configurator)
        {
            base.ConfigureBus(configurator);

//            configurator.UseScheduler()
//
//            configurator.UseDelayedRetry(x =>
//            {
//            });
        }
    }
}
