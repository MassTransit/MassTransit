namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;
    using MassTransit.Contracts;
    using MassTransit.Testing;
    using NUnit.Framework;


    [TestFixture]
    [Category("Flaky")]
    public class Using_a_concurrency_limit_on_a_receive_endpoint :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_allow_reconfiguration()
        {
            IRequestClient<SetConcurrencyLimit> client = Bus.CreateRequestClient<SetConcurrencyLimit>(TestTimeout);

            Response<ConcurrencyLimitUpdated> response = await client.GetResponse<ConcurrencyLimitUpdated>(new
            {
                ConcurrencyLimit = 16,
                Timestamp = DateTime.UtcNow
            }, TestCancellationToken);

            Assert.AreEqual(16, response.Message.ConcurrencyLimit);
        }

        [Test]
        public async Task Should_allow_reconfiguration_of_prefetch_count()
        {
            IRequestClient<SetPrefetchCount> client = Bus.CreateRequestClient<SetPrefetchCount>(TestTimeout);

            for (var i = 0; i < 50; i++)
            {
                await Bus.Publish(new A());

                await Task.Delay(20);
            }

            await client.GetResponse<PrefetchCountUpdated>(new
            {
                PrefetchCount = (ushort)32,
                Timestamp = DateTime.UtcNow,
                QueueName = "input_queue"
            }, TestCancellationToken);

            for (var i = 0; i < 50; i++)
            {
                await Bus.Publish(new A());

                await Task.Delay(20);
            }

            Assert.IsTrue(_consumerA.Consumed.Select<A>().Any());
        }

        ConsumerTestHarness<TestConsumer> _consumerA;
        IRabbitMqBusFactoryConfigurator _configurator;

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            _configurator = configurator;
            _consumerA = RabbitMqTestHarness.Consumer<TestConsumer>();
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _configurator.ManagementEndpoint(mc =>
            {
                configurator.ConnectManagementEndpoint(mc);

                configurator.UseConcurrencyLimit(32, mc);
            });
        }


        class TestConsumer :
            IConsumer<A>
        {
            public Task Consume(ConsumeContext<A> context)
            {
                return Task.CompletedTask;
            }
        }


        class A
        {
        }
    }
}
