namespace MassTransit.Tests
{
    using System;
    using System.Threading.Tasks;
    using Contracts;
    using GreenPipes;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class Using_a_concurrency_limit_on_a_receive_endpoint :
        InMemoryTestFixture
    {
        IInMemoryBusFactoryConfigurator _configurator;

        [Test]
        public async Task Should_allow_reconfiguration()
        {
            var client = Bus.CreateRequestClient<SetConcurrencyLimit>();

            var response = await client.GetResponse<ConcurrencyLimitUpdated>(new
            {
                ConcurrencyLimit = 16,
                Timestamp = DateTime.UtcNow
            });
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            _configurator = configurator;

            base.ConfigureInMemoryBus(configurator);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _configurator.ManagementEndpoint(mc =>
            {
                configurator.UseConcurrencyLimit(32, mc);
            });
        }
    }
}
