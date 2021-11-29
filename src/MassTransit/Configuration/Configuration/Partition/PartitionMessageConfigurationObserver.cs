namespace MassTransit.Configuration
{
    public class PartitionMessageConfigurationObserver :
        ConfigurationObserver,
        IMessageConfigurationObserver
    {
        readonly IPartitioner _partitioner;

        public PartitionMessageConfigurationObserver(IConsumePipeConfigurator configurator, IPartitioner partitioner)
            : base(configurator)
        {
            _partitioner = partitioner;

            Connect(this);
        }

        public void MessageConfigured<TMessage>(IConsumePipeConfigurator configurator)
            where TMessage : class
        {
            var specification = new PartitionMessageSpecification<TMessage>(_partitioner);

            configurator.AddPipeSpecification(specification);
        }

        public override void BatchConsumerConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, Batch<TMessage>> configurator)
        {
            var specification = new PartitionMessageSpecification<Batch<TMessage>>(_partitioner);

            configurator.Message(m => m.AddPipeSpecification(specification));
        }
    }
}
