namespace MassTransit.Clients
{
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;


    public interface IRequestSendEndpoint<T>
        where T : class
    {
        Task<T> CreateMessage(object values, CancellationToken cancellationToken);

        Task Send(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken);
    }
}
