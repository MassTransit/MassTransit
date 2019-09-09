namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;
    using GreenPipes;
    using MassTransit.Contracts;
    using MassTransit.Testing;
    using NUnit.Framework;
    using Util;


    [TestFixture]
    public class Using_a_concurrency_limit_on_a_receive_endpoint :
        RabbitMqTestFixture
    {
        IManagementEndpointConfigurator _management;
        ConsumerTestHarness<TestConsumer> _consumerA;

        protected override void ConfigureRabbitMqBusHost(IRabbitMqBusFactoryConfigurator configurator, IRabbitMqHost host)
        {
            _management = configurator.ManagementEndpoint(host);
            _consumerA = RabbitMqTestHarness.Consumer<TestConsumer>();
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            base.ConfigureRabbitMqReceiveEndpoint(configurator);

            configurator.ConnectManagementEndpoint(_management);

            configurator.UseConcurrencyLimit(32, _management);
        }


        class TestConsumer :
            IConsumer<A>
        {
            public Task Consume(ConsumeContext<A> context)
            {
                return TaskUtil.Completed;
            }
        }


        [Test]
        public async Task Should_allow_reconfiguration()
        {
            var client = Bus.CreateRequestClient<SetConcurrencyLimit>(TestTimeout);

            var response = await client.GetResponse<ConcurrencyLimitUpdated>(new
            {
                ConcurrencyLimit = 16,
                Timestamp = DateTime.UtcNow
            }, TestCancellationToken);

            Assert.AreEqual(16, response.Message.ConcurrencyLimit);
        }

        [Test, Explicit, Category("SlowAF")]
        public async Task Should_allow_reconfiguration_of_prefetch_count()
        {
            var client = Bus.CreateRequestClient<SetPrefetchCount>(TestTimeout);

            for (int i = 0; i < 50; i++)
            {
                await Bus.Publish(new A());

                await Task.Delay(20);
            }

            await client.GetResponse<PrefetchCountUpdated>(new
            {
                PrefetchCount = (ushort)32,
                Timestamp = DateTime.UtcNow,
                QueueName = "input_queue",
            }, TestCancellationToken);

            for (int i = 0; i < 50; i++)
            {
                await Bus.Publish(new A());

                await Task.Delay(20);
            }

            Assert.IsTrue(_consumerA.Consumed.Select<A>().Any());
        }


        class A
        {
        }
    }
}
