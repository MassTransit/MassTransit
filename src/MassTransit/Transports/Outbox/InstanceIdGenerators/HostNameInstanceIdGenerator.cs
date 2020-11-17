using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.Transports.Outbox
{
    public class HostNameInstanceIdGenerator : HostNameBasedIdGenerator
    {
        /// <summary>
        /// Generate the cluster instance id for a the sweeper
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>The clusterwide unique instance id.</returns>
        public override Task<string?> GenerateInstanceId(CancellationToken cancellationToken = default)
        {
            return GetHostName(IdMaxLength, cancellationToken);
        }
    }
}
