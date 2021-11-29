namespace MassTransit.EventHubIntegration.Checkpoints
{
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs.Processor;


    public interface ICheckpointer
    {
        Task Pending(IPendingConfirmation confirmation);
        Task Close(ProcessingStoppedReason stoppedReason);
    }
}
