namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using Middleware;


    public class PartitionSagaSpecification<TSaga> :
        IPipeSpecification<SagaConsumeContext<TSaga>>
        where TSaga : class, ISaga
    {
        readonly PartitionKeyProvider<SagaConsumeContext<TSaga>> _keyProvider;
        readonly IPartitioner _partitioner;

        public PartitionSagaSpecification(IPartitioner partitioner, PartitionKeyProvider<SagaConsumeContext<TSaga>> keyProvider)
        {
            if (partitioner == null)
                throw new ArgumentNullException(nameof(partitioner));
            if (keyProvider == null)
                throw new ArgumentNullException(nameof(keyProvider));

            _partitioner = partitioner;
            _keyProvider = keyProvider;
        }

        public void Apply(IPipeBuilder<SagaConsumeContext<TSaga>> builder)
        {
            builder.AddFilter(new PartitionFilter<SagaConsumeContext<TSaga>>(_keyProvider, _partitioner));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_keyProvider == null)
                yield return this.Failure("KeyProvider", "must not be null");
        }
    }
}
