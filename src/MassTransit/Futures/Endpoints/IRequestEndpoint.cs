namespace MassTransit.Futures.Endpoints
{
    using System.Threading.Tasks;


    public interface IRequestEndpoint<TCommand, out TRequest>
        where TCommand : class
        where TRequest : class
    {
        RequestAddressProvider<TCommand> RequestAddressProvider { set; }

        PendingIdProvider<TRequest> PendingIdProvider { set; }

        Task SendCommand(FutureConsumeContext<TCommand> context);
    }
}
