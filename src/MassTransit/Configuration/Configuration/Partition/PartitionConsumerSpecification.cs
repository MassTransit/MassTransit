namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using Middleware;


    public class PartitionConsumerSpecification<TConsumer> :
        IPipeSpecification<ConsumerConsumeContext<TConsumer>>
        where TConsumer : class
    {
        readonly PartitionKeyProvider<ConsumerConsumeContext<TConsumer>> _keyProvider;
        readonly IPartitioner _partitioner;

        public PartitionConsumerSpecification(IPartitioner partitioner, PartitionKeyProvider<ConsumerConsumeContext<TConsumer>> keyProvider)
        {
            if (partitioner == null)
                throw new ArgumentNullException(nameof(partitioner));
            if (keyProvider == null)
                throw new ArgumentNullException(nameof(keyProvider));

            _partitioner = partitioner;
            _keyProvider = keyProvider;
        }

        public void Apply(IPipeBuilder<ConsumerConsumeContext<TConsumer>> builder)
        {
            builder.AddFilter(new PartitionFilter<ConsumerConsumeContext<TConsumer>>(_keyProvider, _partitioner));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_keyProvider == null)
                yield return this.Failure("KeyProvider", "must not be null");
        }
    }
}
