namespace MassTransit.Clients
{
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;


    public interface IRequestSendEndpoint
    {
        Task<T> CreateMessage<T>(object values, CancellationToken cancellationToken)
            where T : class;

        Task Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class;
    }
}
