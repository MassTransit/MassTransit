namespace MassTransit.Registration.Attachments
{
    using System.Threading;
    using System.Threading.Tasks;


    public interface IBusAttachment
    {
        string Name { get; }
        ValueTask Connect(CancellationToken cancellationToken);
        ValueTask Disconnect(CancellationToken cancellationToken);
    }
}
