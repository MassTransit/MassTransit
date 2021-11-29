namespace MassTransit.AmazonSqsTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public interface IBatcher<in TEntry> :
        IAsyncDisposable
    {
        Task Execute(TEntry entry, CancellationToken cancellationToken = default);
    }
}
