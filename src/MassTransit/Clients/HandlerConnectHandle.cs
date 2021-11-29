namespace MassTransit.Clients
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public interface HandlerConnectHandle<T> :
        HandlerConnectHandle
        where T : class
    {
        Task<Response<T>> Task { get; }
    }


    public interface HandlerConnectHandle :
        ConnectHandle
    {
        void TrySetException(Exception exception);

        void TrySetCanceled(CancellationToken cancellationToken);
    }
}
