using System;

namespace MassTransit.RabbitMqTransport
{
    public interface IExchangeTypeDeterminer
    {
        string getTypeForExchangeName(String exchangeName);
    }
}