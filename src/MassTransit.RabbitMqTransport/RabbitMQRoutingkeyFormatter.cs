using System;

namespace MassTransit.RabbitMqTransport
{
    public class RabbitMqRoutingkeyFormatter: IRoutingkeyFormatter
    {
        public string createRoutingkeyForType(Type messageType)
        {
            return "";
        }
    }
}