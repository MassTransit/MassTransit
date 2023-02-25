namespace MassTransit.EventHubIntegration.Checkpoints
{
    using Azure.Messaging.EventHubs.Processor;


    // ReSharper disable NotAccessedField.Local
    public readonly struct PartitionOffset
    {
        readonly string _partitionId;
        readonly long _offset;

        PartitionOffset(string partitionId, long offset)
        {
            _partitionId = partitionId;
            _offset = offset;
        }

        public override string ToString()
        {
            return $"{_partitionId}/{_offset}";
        }

        public static implicit operator PartitionOffset(in ProcessEventArgs args)
        {
            return new PartitionOffset(args.Partition.PartitionId, args.Data.Offset);
        }
    }
}
