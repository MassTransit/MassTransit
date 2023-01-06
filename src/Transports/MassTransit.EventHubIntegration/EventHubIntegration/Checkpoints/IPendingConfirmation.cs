namespace MassTransit.EventHubIntegration.Checkpoints
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs.Consumer;


    public interface IPendingConfirmation
    {
        PartitionContext Partition { get; }

        long Offset { get; }

        Task Confirmed { get; }

        void Complete();
        void Faulted(Exception exception);
        void Faulted(string message);
        void Canceled(CancellationToken cancellationToken);

        Task Checkpoint(CancellationToken cancellationToken);
    }
}
