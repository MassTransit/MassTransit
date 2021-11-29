namespace MassTransit.Tests
{
    using System;
    using System.Threading.Tasks;
    using Contracts;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class Using_a_concurrency_limit_on_a_receive_endpoint :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_allow_reconfiguration()
        {
            IRequestClient<SetConcurrencyLimit> client = Bus.CreateRequestClient<SetConcurrencyLimit>();

            Response<ConcurrencyLimitUpdated> response = await client.GetResponse<ConcurrencyLimitUpdated>(new
            {
                ConcurrencyLimit = 16,
                Timestamp = DateTime.UtcNow
            });
        }

        IInMemoryBusFactoryConfigurator _configurator;

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
