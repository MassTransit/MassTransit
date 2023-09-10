namespace MassTransit
{
    using System.Threading;
    using System.Threading.Tasks;
    using Courier.Contracts;


    public interface IRoutingSlipExecutor
    {
        /// <summary>
        /// Execute a routing slip
        /// </summary>
        /// <param name="routingSlip"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Execute(RoutingSlip routingSlip, CancellationToken cancellationToken = default);
    }
}
