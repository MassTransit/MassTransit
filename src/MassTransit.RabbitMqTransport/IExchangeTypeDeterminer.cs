using System;

namespace MassTransit.RabbitMqTransport
{
    public interface IExchangeTypeDeterminer
    {
        string GetTypeForExchangeName(String exchangeName);
    }
}