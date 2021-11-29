namespace MassTransit
{
    using System;


    public static class EndpointConvention
    {
        /// <summary>
        /// Map the message type to the specified address
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="destinationAddress"></param>
        public static void Map<T>(Uri destinationAddress)
            where T : class
        {
            EndpointConventionCache<T>.Map(destinationAddress);
        }

        /// <summary>
        /// Map the message type to the endpoint returned by the specified method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpointAddressProvider"></param>
        public static void Map<T>(EndpointAddressProvider<T> endpointAddressProvider)
            where T : class
        {
            EndpointConventionCache<T>.Map(endpointAddressProvider);
        }

        public static bool TryGetDestinationAddress<T>(out Uri destinationAddress)
            where T : class
        {
            return EndpointConventionCache<T>.TryGetEndpointAddress(out destinationAddress);
        }

        public static bool TryGetDestinationAddress(Type messageType, out Uri destinationAddress)
        {
            return EndpointConventionCache.TryGetEndpointAddress(messageType, out destinationAddress);
        }
    }
}
