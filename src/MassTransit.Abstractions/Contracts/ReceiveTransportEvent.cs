namespace MassTransit
{
    using System;


    public interface ReceiveTransportEvent
    {
        /// <summary>
        /// The input address of the receive endpoint
        /// </summary>
        Uri InputAddress { get; }
    }
}
