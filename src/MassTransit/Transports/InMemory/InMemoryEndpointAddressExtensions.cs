namespace MassTransit.Transports.InMemory
{
    using System;
    using System.Linq;


    public static class InMemoryEndpointAddressExtensions
    {
        /// <summary>
        /// Returns the queue/exchange name from the address
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static string GetQueueOrExchangeName(this Uri address)
        {
            return address?.AbsolutePath.Split('/').LastOrDefault();
        }
    }
}
