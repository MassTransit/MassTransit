using MassTransit.Context;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.Transports.OnRamp
{
    public abstract class HostNameBasedIdGenerator : IInstanceIdGenerator
    {
        protected const int IdMaxLength = 50;

        protected HostNameBasedIdGenerator()
        {
        }

        /// <summary>
        /// Generate the cluster instance id for a the sweeper
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns> The clusterwide unique instance id.
        /// </returns>
        public abstract Task<string> GenerateInstanceId(CancellationToken cancellationToken = default);

        protected async Task<string> GetHostName(
            int maxLength,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var hostAddress = await GetHostAddress(cancellationToken).ConfigureAwait(false);
                string hostName = hostAddress.HostName;
                if (hostName != null && hostName.Length > maxLength)
                {
                    string newName = hostName.Substring(0, maxLength);
                    LogContext.Info?.Log("Host name '{0}' was too long, shortened to '{1}'", hostName, newName);
                    hostName = newName;
                }
                return hostName;
            }
            catch (Exception e)
            {
                throw new OnRampException("Couldn't get host name!", e);
            }
        }

        protected virtual async Task<IPHostEntry> GetHostAddress(
            CancellationToken cancellationToken = default)
        {
            var hostEntry = await Dns.GetHostEntryAsync(Dns.GetHostName()).ConfigureAwait(false);
            var firstAddressEntry = await Dns.GetHostEntryAsync(hostEntry.AddressList[0].ToString()).ConfigureAwait(false);
            return firstAddressEntry;
        }
    }
}
