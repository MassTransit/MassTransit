using System;

namespace MassTransit.RabbitMqTransport
{
    public interface IRoutingKeyFormatter
    {
        string CreateRoutingKeyForType(Type messageType);
    }
}