namespace MassTransit
{
    using System;


    public interface HostReady
    {
        /// <summary>
        /// The Host address
        /// </summary>
        Uri HostAddress { get; }

        /// <summary>
        /// The receive endpoints that were started on the host
        /// </summary>
        ReceiveEndpointReady[] ReceiveEndpoints { get; }

        /// <summary>
        /// The riders that were started on the host
        /// </summary>
        RiderReady[] Riders { get; }
    }
}
