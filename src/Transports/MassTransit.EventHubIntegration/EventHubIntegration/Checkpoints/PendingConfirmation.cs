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
        readonly string _eventHubName;
        readonly TaskCompletionSource<long> _source;
        ProcessEventArgs _eventArgs;

        public PendingConfirmation(string eventHubName, ProcessEventArgs eventArgs)
        {
            _eventHubName = eventHubName;
            _eventArgs = eventArgs;
            _source = TaskUtil.GetTask<long>();
        }

        Uri Topic => new Uri($"topic:{_eventHubName}");

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

        public Task Checkpoint(CancellationToken cancellationToken = default)
        {
            return _eventArgs.UpdateCheckpointAsync(cancellationToken);
        }
    }
}
