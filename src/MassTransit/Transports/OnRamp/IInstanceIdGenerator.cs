using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.Transports.OnRamp
{
    public interface IInstanceIdGenerator
    {
        /// <summary>
        /// Generate the cluster instance id for a the sweeper
        /// </summary>
        /// <returns> The clusterwide unique instance id.</returns>
        Task<string> GenerateInstanceId(CancellationToken cancellationToken = default);
    }
}
