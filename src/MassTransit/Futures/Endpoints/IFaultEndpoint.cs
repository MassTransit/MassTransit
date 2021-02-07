namespace MassTransit.Futures.Endpoints
{
    using System.Threading.Tasks;


    public interface IFaultEndpoint<in TInput>
        where TInput : class
    {
        Task SendFault(FutureConsumeContext<TInput> context, params FutureSubscription[] subscriptions);
    }


    public interface IFaultEndpoint
    {
        Task SendFault(FutureConsumeContext context, params FutureSubscription[] subscriptions);
    }
}