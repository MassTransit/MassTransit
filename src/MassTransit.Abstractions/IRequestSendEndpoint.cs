namespace MassTransit
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public interface IRequestSendEndpoint<T>
        where T : class
    {
        Task<T> Send(Guid requestId, object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken);

        Task Send(Guid requestId, T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken);
    }
}
