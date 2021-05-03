namespace MassTransit.KafkaIntegration.Configurators
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Context;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Builders;
    using GreenPipes.Configurators;


    public class CheckpointPipeConfiguration :
        ICheckpointPipeConfigurator,
        ISpecification
    {
        readonly IBuildPipeConfigurator<CheckpointContext> _configurator;

        public CheckpointPipeConfiguration()
        {
            _configurator = new PipeConfigurator<CheckpointContext>();
        }

        public void AddPipeSpecification(IPipeSpecification<CheckpointContext> specification)
        {
            _configurator.AddPipeSpecification(specification);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _configurator.Validate();
        }

        public IPipe<CheckpointContext> CreatePipe<TKey, TValue>(IConsumer<TKey, TValue> consumer)
        {
            _configurator.UseFilter(new ConsumerCheckpointFilter<TKey, TValue>(consumer));

            return _configurator.Build();
        }


        class ConsumerCheckpointFilter<TKey, TValue> :
            IFilter<CheckpointContext>
        {
            readonly IConsumer<TKey, TValue> _consumer;

            public ConsumerCheckpointFilter(IConsumer<TKey, TValue> consumer)
            {
                _consumer = consumer;
            }

            public async Task Send(CheckpointContext context, IPipe<CheckpointContext> next)
            {
                LogContext.Debug?.Log("Partition: {PartitionId} updating checkpoint with offset: {Offset}", context.Partition, context.Offset);
                _consumer.Commit(new[] {new TopicPartitionOffset(context.Topic, context.Partition, context.Offset)});

                await next.Send(context).ConfigureAwait(false);
            }

            public void Probe(ProbeContext context)
            {
            }
        }
    }
}
