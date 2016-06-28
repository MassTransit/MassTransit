using System;

namespace MassTransit.RabbitMqTransport
{
    public interface IRoutingkeyFormatter
    {
        string createRoutingkeyForType(Type messageType);
    }
}