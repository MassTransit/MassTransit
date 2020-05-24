namespace MassTransit.Registration.Attachments
{
    using System.Threading;
    using System.Threading.Tasks;


    public interface IBusAttachment
    {
        string Name { get; }
        Task Connect(CancellationToken cancellationToken);
        Task Disconnect(CancellationToken cancellationToken);
    }
}
