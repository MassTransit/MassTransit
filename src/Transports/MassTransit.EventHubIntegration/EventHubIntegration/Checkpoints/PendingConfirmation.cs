namespace MassTransit.EventHubIntegration.Checkpoints
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs.Consumer;
    using Azure.Messaging.EventHubs.Processor;
    using Util;


    public class PendingConfirmation :
        IPendingConfirmation
    {
        readonly TaskCompletionSource<long> _source;
        ProcessEventArgs _eventArgs;

        public PendingConfirmation(ProcessEventArgs eventArgs)
        {
            _eventArgs = eventArgs;
            _source = TaskUtil.GetTask<long>();
        }

        Uri Topic => new Uri($"topic:{Partition.EventHubName}");

        public PartitionContext Partition => _eventArgs.Partition;
        public long Offset => _eventArgs.Data.Offset;

        public Task Confirmed => _source.Task;

        public void Complete()
        {
            _source.TrySetResult(Offset);
        }

        public void Faulted(Exception exception)
        {
            _source.TrySetException(new MessageNotConsumedException(Topic, "Message not consumed", exception));
        }

        public void Faulted(string message)
        {
            _source.TrySetException(new ArgumentException(message));
        }

        public void Canceled(CancellationToken cancellationToken)
        {
            _source.TrySetCanceled(cancellationToken);
        }

        public Task Checkpoint(CancellationToken cancellationToken)
        {
            return _eventArgs.UpdateCheckpointAsync(cancellationToken);
        }
    }
}
