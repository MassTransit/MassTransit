namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using Middleware;


    public class PartitionMessageSpecification<T> :
        IPipeSpecification<ConsumeContext<T>>
        where T : class
    {
        readonly IPartitioner _partitioner;
        PartitionKeyProvider<ConsumeContext<T>> _keyProvider;

        public PartitionMessageSpecification(IPartitioner partitioner)
        {
            _partitioner = partitioner;
        }

        public void Apply(IPipeBuilder<ConsumeContext<T>> builder)
        {
            if (_keyProvider == null)
                throw new ConfigurationException($"The partition key provider was not found for message type: {TypeCache<T>.ShortName}");

            builder.AddFilter(new PartitionFilter<ConsumeContext<T>>(_keyProvider, _partitioner));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (GlobalTopology.Send.GetMessageTopology<T>().TryGetConvention(out ICorrelationIdMessageSendTopologyConvention<T> convention)
                && convention.TryGetMessageCorrelationId(out IMessageCorrelationId<T> messageCorrelationId))
            {
                _keyProvider = context => messageCorrelationId.TryGetCorrelationId(context.Message, out var correlationId)
                    ? correlationId.ToByteArray()
                    : default(Guid).ToByteArray();
            }
            else
                yield return this.Failure("Partition", TypeCache<T>.ShortName, "A CorrelationId convention for this message type was not found.");
        }
    }
}
