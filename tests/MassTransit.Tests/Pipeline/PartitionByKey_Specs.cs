namespace MassTransit.Tests.Pipeline
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class Partitioning_a_consumer_by_key :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_use_a_partitioner_for_consistency()
        {
            await Task.WhenAll(Enumerable.Range(0, Limit).Select(index => Bus.Publish(new PartitionedMessage { CorrelationId = NewId.NextGuid() })));

            var count = await _completed.Task;

            Assert.AreEqual(Limit, count);

            Console.WriteLine("Processed: {0}", count);
        }

        TaskCompletionSource<int> _completed;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _completed = GetTask<int>();

            configurator.Consumer(() => new PartitionedConsumer(_completed), x =>
            {
                x.Message<PartitionedMessage>(m =>
                {
                    m.UsePartitioner(8, context => context.Message.CorrelationId);
                });
            });
        }

        const int Limit = 100;


        class PartitionedConsumer :
            IConsumer<PartitionedMessage>
        {
            static int _count;
            readonly TaskCompletionSource<int> _completed;

            public PartitionedConsumer(TaskCompletionSource<int> completed)
            {
                _completed = completed;
            }

            public Task Consume(ConsumeContext<PartitionedMessage> context)
            {
                if (Interlocked.Increment(ref _count) == Limit)
                    _completed.TrySetResult(Limit);

                return Task.CompletedTask;
            }
        }


        class PartitionedMessage
        {
            public Guid CorrelationId { get; set; }
        }
    }


    [TestFixture]
    public class Partition_by_convention_with_matching_property :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_use_a_partitioner_for_consistency()
        {
            await Task.WhenAll(Enumerable.Range(0, Limit).Select(index => Bus.Publish(new PartitionedMessage { CorrelationId = NewId.NextGuid() })));

            var count = await _completed.Task;

            Assert.AreEqual(Limit, count);

            Console.WriteLine("Processed: {0}", count);
        }

        TaskCompletionSource<int> _completed;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _completed = GetTask<int>();

            configurator.UseMessagePartitioner(8);

            configurator.Consumer(() => new PartitionedConsumer(_completed));
        }

        const int Limit = 100;


        class PartitionedConsumer :
            IConsumer<PartitionedMessage>
        {
            static int _count;
            readonly TaskCompletionSource<int> _completed;

            public PartitionedConsumer(TaskCompletionSource<int> completed)
            {
                _completed = completed;
            }

            public Task Consume(ConsumeContext<PartitionedMessage> context)
            {
                if (Interlocked.Increment(ref _count) == Limit)
                    _completed.TrySetResult(Limit);

                return Task.CompletedTask;
            }
        }


        class PartitionedMessage
        {
            public Guid CorrelationId { get; set; }
        }
    }


    [TestFixture]
    public class Partition_by_convention_without_property
    {
        [Test]
        public async Task Should_fail_with_exception()
        {
            Assert.That(() =>
                Bus.Factory.CreateUsingInMemory(cfg =>
                {
                    cfg.ReceiveEndpoint("input-queue", e =>
                    {
                        e.UseMessagePartitioner(8);

                        e.Consumer(() => new UnpartitionedConsumer());
                    });
                }), Throws.TypeOf<ConfigurationException>().With.Message.Contains("A CorrelationId convention for this message type was not found"));
        }


        class UnpartitionedConsumer :
            IConsumer<UnpartitionedMessage>
        {
            public Task Consume(ConsumeContext<UnpartitionedMessage> context)
            {
                return Task.CompletedTask;
            }
        }


        class UnpartitionedMessage
        {
            public Guid TacoId { get; set; }
        }
    }
}
