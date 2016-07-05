using System;

namespace MassTransit.RabbitMqTransport
{
    /// <summary>
    /// The formatter that is used for creating the default routing logic in MT3
    /// </summary>
    public class MasstransitRoutingKeyFormatter: IRoutingKeyFormatter
    {
        /// <summary>
        /// The method that is called when a routingkey is required
        /// </summary>
        /// <returns> 
        /// No routingkeys are used in the default MT3 routing logic therefore "" is returned
        /// </returns>
        public string CreateRoutingKeyForType(Type messageType)
        {
            return "";
        }
    }
}