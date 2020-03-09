namespace MassTransit.Contracts
{
    using System;


    /// <summary>
    /// Service endpoint information, for a given message
    /// </summary>
    public interface ServiceInfo
    {
        /// <summary>
        /// The destination address for the service endpoint
        /// </summary>
        Uri ServiceAddress { get; }

        /// <summary>
        /// The service's capabilities, which may include control plane requests, commands, and events
        /// </summary>
        ServiceCapability[] Capabilities { get; }
    }
}
