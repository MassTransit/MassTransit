namespace MassTransit
{
    using System;


    public interface ReceiveEndpointEvent
    {
        /// <summary>
        /// The input address of the receive endpoint
        /// </summary>
        Uri InputAddress { get; }

        /// <summary>
        /// The receive endpoint upon which the event occurred
        /// </summary>
        IReceiveEndpoint ReceiveEndpoint { get; }
    }
}
