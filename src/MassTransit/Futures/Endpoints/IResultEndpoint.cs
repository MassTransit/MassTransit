namespace MassTransit.Futures.Endpoints
{
    using System.Threading.Tasks;


    public interface IResultEndpoint<in TResult>
        where TResult : class
    {
        Task SendResponse(FutureConsumeContext<TResult> context, params FutureSubscription[] subscriptions);
    }


    public interface IResultEndpoint
    {
        Task SendResponse(FutureConsumeContext context, params FutureSubscription[] subscriptions);
    }
}
