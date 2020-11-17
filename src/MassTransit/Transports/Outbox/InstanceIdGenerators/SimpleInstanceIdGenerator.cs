using System;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.Transports.Outbox
{
    public class SimpleInstanceIdGenerator : HostNameBasedIdGenerator
    {
        // assume ticks to be at most 20 chars long
        private const int HostNameMaxLength = IdMaxLength - 20;

        public override async Task<string> GenerateInstanceId(CancellationToken cancellationToken = default)
        {
            var hostName = await GetHostName(HostNameMaxLength, cancellationToken).ConfigureAwait(false);
            return hostName + DateTime.UtcNow.Ticks;
        }
    }
}
