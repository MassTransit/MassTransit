namespace MassTransit.Futures.Endpoints
{
    using System.Threading.Tasks;


    public interface IRoutingSlipExecutor<in TInput>
        where TInput : class
    {
        bool TrackRoutingSlip { set; }
        Task Execute(FutureConsumeContext<TInput> context);
    }
}
