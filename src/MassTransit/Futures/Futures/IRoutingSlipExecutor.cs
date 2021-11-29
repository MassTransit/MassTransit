namespace MassTransit.Futures
{
    using System.Threading.Tasks;


    public interface IRoutingSlipExecutor<in TInput>
        where TInput : class
    {
        bool TrackRoutingSlip { set; }
        Task Execute(BehaviorContext<FutureState, TInput> context);
    }
}
