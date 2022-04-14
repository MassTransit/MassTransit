namespace MassTransit.Transports
{
    using System.Threading;
    using System.Threading.Tasks;


    public interface ITransportSendEndpoint :
        ISendEndpoint
    {
        Task<SendContext<T>> CreateSendContext<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class;
    }
}
