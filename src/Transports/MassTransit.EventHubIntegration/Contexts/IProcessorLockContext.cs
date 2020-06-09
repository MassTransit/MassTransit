namespace MassTransit.EventHubIntegration.Contexts
{
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs.Processor;


    public interface IProcessorLockContext
    {
        Task Complete(ProcessEventArgs eventArgs);
    }
}
