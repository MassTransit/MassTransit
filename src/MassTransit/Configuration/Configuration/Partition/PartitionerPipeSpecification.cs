namespace MassTransit.Configuration
{
    using System.Collections.Generic;
    using Middleware;


    public class PartitionerPipeSpecification<T> :
        IPipeSpecification<T>
        where T : class, PipeContext
    {
        readonly PartitionKeyProvider<T> _keyProvider;
        readonly int _partitionCount;
        readonly IPartitioner _partitioner;

        public PartitionerPipeSpecification(PartitionKeyProvider<T> keyProvider, int partitionCount)
        {
            _keyProvider = keyProvider;
            _partitionCount = partitionCount;
        }

        public PartitionerPipeSpecification(PartitionKeyProvider<T> keyProvider, IPartitioner partitioner)
        {
            _keyProvider = keyProvider;
            _partitioner = partitioner;
        }

        public void Apply(IPipeBuilder<T> builder)
        {
            var partitioner = _partitioner ?? new Partitioner(_partitionCount, new Murmur3UnsafeHashGenerator());

            builder.AddFilter(new PartitionFilter<T>(_keyProvider, partitioner));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_keyProvider == null)
                yield return this.Failure("KeyProvider", "must not be null");
            if (_partitioner == null && _partitionCount < 1)
                yield return this.Failure("PartitionCount", "must be >= 1");
        }
    }
}
