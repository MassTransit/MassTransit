namespace MassTransit.KafkaIntegration.Checkpoints
{
    using System;
    using System.Threading.Tasks;


    public interface ICheckpointer :
        IAsyncDisposable
    {
        Task Pending(IPendingConfirmation confirmation);
    }
}
