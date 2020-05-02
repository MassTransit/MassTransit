namespace MassTransit.AmazonSqsTransport.Contexts
{
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;


    public interface IBatcher<in TEntry> :
        IAsyncDisposable
    {
        Task Execute(TEntry entry, CancellationToken cancellationToken = default);
    }
}
